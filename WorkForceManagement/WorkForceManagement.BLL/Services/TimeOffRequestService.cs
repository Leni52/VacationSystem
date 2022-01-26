using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkForceManagement.BLL.Exceptions;
using WorkForceManagement.BLL.Services.Interfaces;
using WorkForceManagement.DAL;
using WorkForceManagement.DAL.Entities;
using WorkForceManagement.DAL.Repositories;

namespace WorkForceManagement.BLL.Services
{
    public class TimeOffRequestService : ITimeOffRequestService
    {
        private readonly IRepository<TimeOffRequest> _timeOffRequestRepository;
        private readonly IUserService _userService;
        private readonly ITeamService _teamService;
        private readonly IMailService _mailService;
        private readonly List<DateTime> officialDaysOff = Calendar.GenerateCalendar();

        public TimeOffRequestService(
            IRepository<TimeOffRequest> timeOffRequestRepository,
            IUserService userService,
            ITeamService teamService,
            IMailService mailService)
        {
            _timeOffRequestRepository = timeOffRequestRepository;
            _userService = userService;
            _teamService = teamService;
            _mailService = mailService;
        }

        public async Task CreateTimeOffRequest(TimeOffRequest request, User currentUser)
        {
            ValidateTimeOffRequestDates(request.StartDate, request.EndDate, currentUser);
            request.WorkingDaysOff = ValidateDaysOff(request.StartDate, request.EndDate);

            currentUser.DaysOff -= request.WorkingDaysOff;

            request.Status = TimeOffRequestStatus.Created;
            request.CreatorId = currentUser.Id;
            request.UpdaterId = currentUser.Id;
            var approvers = GetApprovers(currentUser);

            approvers.ForEach(user => request.Approvers.Add(user));
            approvers.ForEach(user => user.TimeOffRequestsToApprove.Add(request));
            currentUser.CreatedTimeOffRequests.Add(request);

            await _timeOffRequestRepository.CreateOrUpdate(request);
            await CheckTimeOffRequest(request.Id);
        }

        private List<User> GetApprovers(User currentUser)
        { // gets all the approvers and checks if they themselves are not in a timeoff
            var potentialApprovers = currentUser.Teams.Select(team => team.TeamLeader).ToList();
            var validatedApprovers = new List<User>();

            foreach (var user in potentialApprovers)
            {
                bool teamLeaderIsAway = user.CreatedTimeOffRequests.Any(x => x.Status == TimeOffRequestStatus.Approved && x.StartDate.Date <= DateTime.Now.Date
                && x.EndDate.Date >= DateTime.Now.Date);

                if (!teamLeaderIsAway) // if approver is not away today add him as validatedApprover
                    validatedApprovers.Add(user);
            }
            return validatedApprovers;
        }

        private void ValidateTimeOffRequestDates(DateTime startDate, DateTime endDate, User user)
        {
            if (startDate > endDate) // you cant create time off 1 day before start
                throw new InvalidDatesException("Invalid time off request dates, the start date should be earlier or equal to end date");
            if(startDate.Date < DateTime.Now.Date)
                throw new InvalidDatesException("Invalid time off request dates, the start date should be at least be no earlier than today");

            int requestedDays = ValidateDaysOff(startDate, endDate);
            int totalDays = user.DaysOff;
            if (requestedDays > totalDays)
                throw new InvalidDatesException("The number of requested days exceeds the maximum number available");

            bool isOverlapping = user.CreatedTimeOffRequests.Any(
                timeOff =>
                timeOff.Status != TimeOffRequestStatus.Rejected &&
                timeOff.Status != TimeOffRequestStatus.Cancelled &&
                timeOff.StartDate < endDate &&
                startDate < timeOff.EndDate
                );

            if (isOverlapping) // if current user already has a timeoffrequest thats not rejected and is overlapping with the current timeOffRequest
                throw new OverlappingTimeOffRequestsException($"User with id: {user.Id}, already has a time off request thats overlapping with the current request!");
        }

        private int ValidateDaysOff(DateTime startDate, DateTime endDate)
        {
            return Enumerable.Range(0, 1 + endDate.Subtract(startDate).Days)
                .Select(offset => startDate.AddDays(offset))
                .Where(day => !IsDayOfficialDayOff(day) && !IsDayWeekend(day))
                .Count();
        }

        private bool IsDayOfficialDayOff(DateTime day)
        {
            return officialDaysOff.Any(date => date.Date == day.Date);
        }

        private bool IsDayWeekend(DateTime day)
        {
            return day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday;
        }

        public async Task DeleteTimeOffRequest(Guid Id)
        {
            var request = await _timeOffRequestRepository.Get(Id);
            if (request != null)
            {
                await _timeOffRequestRepository.Remove(request);
            }
            else
                throw new ItemDoesNotExistException();
        }

        public async Task<List<TimeOffRequest>> GetAllRequests()
        {
            return await _timeOffRequestRepository.All();
        }

        public async Task<TimeOffRequest> GetTimeOffRequest(Guid Id)
        {
            var request = await _timeOffRequestRepository.Get(Id);
            if (request == null)
            {
                throw new ItemDoesNotExistException();
            }
            return request;
        }

        public async Task<TimeOffRequest> UpdateTimeOffRequest(Guid timeOffRequestId, TimeOffRequest timeOffRequest, string currentUserId)

        {
            ValidateTimeOffRequestDates(timeOffRequest.StartDate, timeOffRequest.EndDate, await _userService.GetUserById(Guid.Parse(currentUserId)));
            int daysOff = ValidateDaysOff(timeOffRequest.StartDate, timeOffRequest.EndDate);

            TimeOffRequest requestToUpdate = await _timeOffRequestRepository.Get(timeOffRequestId);
            if (requestToUpdate == null)
            {
                throw new ItemDoesNotExistException();
            }
            requestToUpdate.WorkingDaysOff = daysOff;
            requestToUpdate.ChangeDate = DateTime.Now;
            requestToUpdate.UpdaterId = currentUserId;
            requestToUpdate.Type = timeOffRequest.Type;
            requestToUpdate.Description = timeOffRequest.Description;
            requestToUpdate.StartDate = timeOffRequest.StartDate.Date;
            requestToUpdate.EndDate = timeOffRequest.EndDate.Date;

            await _timeOffRequestRepository.CreateOrUpdate(requestToUpdate);
            return requestToUpdate;
        }

        public async Task<List<TimeOffRequest>> GetMyRequests(Guid userId)
        {
            var requests = await _timeOffRequestRepository.All();
            return requests.Where(request => request.CreatorId == userId.ToString()).ToList();
        }

        public async Task AnswerTimeOffRequest(Guid id, bool isApproved, User currentUser, string reason)
        {
            var request = await GetTimeOffRequest(id);

            if (request.Status == TimeOffRequestStatus.Approved ||
                request.Status == TimeOffRequestStatus.Rejected ||
                request.Status == TimeOffRequestStatus.Cancelled)
            { // the time off request has already been decided
                throw new TimeOffRequestIsClosedException($"Time off request with id:{id}, is already closed");
            }

            bool currentUserIsApprover = request.Approvers.Any(user => user.Id == currentUser.Id);
            if (currentUserIsApprover == false)
                throw new UserIsntApproverException($"User with Id:{currentUser.Id}, cant approve this Time Off Request");

            request.ChangeDate = DateTime.Now.Date;
            request.UpdaterId = currentUser.Id;
            if(reason != null)
            {
                request.Reason = reason;
            }

            if (request.Status == TimeOffRequestStatus.Created)
            {
                request.Status = TimeOffRequestStatus.Awaiting;
            }

            if (isApproved)
            {
                await ApproveTimeOffRequest(request, currentUser);
            }
            else
            {
                await RejectTimeOffRequest(request);
            }
        }

        private async Task RejectTimeOffRequest(TimeOffRequest request)
        {
            request.Status = TimeOffRequestStatus.Rejected;

            request.Requester.DaysOff += request.WorkingDaysOff;

            await SendMailToRequesterRejectedRequest(request.Id);
            await NotifyApproversOnDecision(TimeOffRequestStatus.Rejected, request);

            // removes the request from the approvers timeOffRequestsToApprove
            request.Approvers.ForEach(approver => approver.TimeOffRequestsToApprove.Remove(request));

            await _timeOffRequestRepository.SaveChanges();
        }

        private async Task ApproveTimeOffRequest(TimeOffRequest request, User currentUser)
        {
            request.AlreadyApproved.Add(currentUser);
            await _timeOffRequestRepository.SaveChanges();
            //email to requester
            await SendMailToRequesterApprovedRequest(request.Id);
            await CheckTimeOffRequest(request.Id);
        }

        public async Task<string> CheckTimeOffRequest(Guid id)
        {
            var request = await GetTimeOffRequest(id);

            if (request == null)
            {
                throw new ItemDoesNotExistException();
            }
            if (request.Status == TimeOffRequestStatus.Rejected)
            {
                return "Rejected";
            }
            else if (request.Status == TimeOffRequestStatus.Approved)
            {
                return "Approved";
            }
            else if (request.Status == TimeOffRequestStatus.Cancelled)
            {
                return "Cancelled";
            }

            int numberOfApprovers = request.Approvers.ToList().Count; // get all needed approvals

            if (numberOfApprovers == request.AlreadyApproved.ToList().Count ||
                request.Type == TimeOffRequestType.SickLeave) // compare with current approvals OR it's sick leave
            {
                request.Status = TimeOffRequestStatus.Approved;
                await NotifyApproversOnDecision(TimeOffRequestStatus.Approved, request);
                await SendMailToRequesterApprovedRequest(request.Id);
                // To all approvers, removing the request from toApprove and adding it to Approved list
                var approvers = request.Approvers.ToList();

                approvers.ForEach(approver => approver.TimeOffRequestsToApprove.Remove(request));
                approvers.ForEach(approver => approver.TimeOffRequestsApproved.Add(request));

                request.Requester.DaysOff -= request.WorkingDaysOff;
                //subtract the available days since the request is approved
                await _timeOffRequestRepository.SaveChanges();

                await NotifyTeamMembersLeaderIsOOO(request);
                return "Approved";
            }
            else
            {// sending notifications to leaders which nave not yet confirmed, in case of call after creaton it's sent to all team leaders
                var approvers = request.Approvers.Except(request.AlreadyApproved).ToList();

                foreach (var u in approvers)
                {
                    await _mailService.SendEmail(new MailRequest()
                    {
                        ToEmail = u.Email,
                        Subject = $"{request.Requester.UserName} is requesting a TimeOff!",
                        Body = $"{request.Requester.UserName} is requesting a TOR between the dates {request.StartDate.ToShortDateString()} and {request.EndDate.ToShortDateString()}!\n" +
                        $"Total days off: {request.WorkingDaysOff}"
                    });
                }

                await _timeOffRequestRepository.SaveChanges();
                return request.Status == TimeOffRequestStatus.Awaiting ? "Awaiting" : "Created";
            }
        }

        private async Task SendMailToRequesterApprovedRequest(Guid id)
        {
            var request = await _timeOffRequestRepository.Get(id);
            //email to requester
            var mailRequest = new MailRequest()
            {
                Subject = "Approved request.",
                Body = $"Your request between {request.StartDate.ToShortDateString()}" +
                $" and {request.EndDate.ToShortDateString()}  has been approved.",
                ToEmail = request.Requester.Email
            };

            await _mailService.SendEmail(mailRequest);
        }

        private async Task SendMailToRequesterRejectedRequest(Guid id)
        {
            var request = await _timeOffRequestRepository.Get(id);
            //email to requester
            var mailRequest = new MailRequest()
            {
                Subject = "Rejected request.",
                Body = $"Your request with start date: {request.StartDate.ToShortDateString()} " +
                $" and end date: {request.EndDate.ToShortDateString()} has been rejected.",
                ToEmail = request.Requester.Email
            };
            await _mailService.SendEmail(mailRequest);
        }

        private async Task NotifyTeamMembersLeaderIsOOO(TimeOffRequest request)
        {
            var users = await _userService.GetUsersUnderTeamLeader(request.Requester);

            if (users.Count != 0)
            {
                foreach (var user in users)
                {
                    await _mailService.SendEmail(new MailRequest()
                    {
                        ToEmail = user.Email,
                        Subject = "TeamLeader OOO!",
                        Body = $"{request.Requester.UserName} is OOO until {request.EndDate.ToShortDateString()}!"
                    });
                }
            }
        }

        private async Task NotifyApproversOnDecision(TimeOffRequestStatus status, TimeOffRequest request)
        {
            string subject = "";
            string body = "";
            if (status == TimeOffRequestStatus.Approved)
            {
                subject = "Time off Request Approved";
                body = $"Time off request by: {request.Requester.UserName} with start date: {request.StartDate.ToShortDateString()} and end date: {request.EndDate.ToShortDateString()} is APPROVED";
            }
            else if (status == TimeOffRequestStatus.Rejected)
            {
                subject = "Time off request Rejected";
                body = $"Time off request by: {request.Requester.UserName} with start date: {request.StartDate.ToShortDateString()} and end date: {request.EndDate.ToShortDateString()} is REJECTED";
            }
            else if (status == TimeOffRequestStatus.Cancelled)
            {
                subject = "Time off request Cancelled";
                body = $"Time off request by: {request.Requester.UserName} with start date: {request.StartDate.ToShortDateString()} and end date: {request.EndDate.ToShortDateString()} has been CANCELLED";
            }

            var approvers = request.Approvers.ToList();

            foreach (var approver in approvers)
            {
                await _mailService.SendEmail(new MailRequest()
                {
                    ToEmail = approver.Email,
                    Subject = subject,
                    Body = body
                });
            }
        }

        public async Task<List<User>> GetMyColleguesTimeOffRequests(User currentUser)
        {
            var teams = _userService.GetUserTeams(currentUser);
            var members = new List<User>();
            var membersOnVacation = new List<User>();

            foreach (var team in teams)
            {
                members = await _teamService.GetAllTeamMembers(team.Id);
            }

            foreach (var user in members)
            {
                if ((user.CreatedTimeOffRequests.Where(t => t.Status == TimeOffRequestStatus.Approved).Any()) &&
                    (user.CreatedTimeOffRequests.Where(t => t.EndDate >= DateTime.Now).Any()))
                {
                    membersOnVacation.Add(user);
                }
            }

            return membersOnVacation;
        }

        public async Task CancelTimeOffRequest(Guid id)
        {
            var request = await _timeOffRequestRepository.Get(id);

            if (request == null)
            {
                throw new ItemDoesNotExistException($"TimeOffRequest with id: {id} doesn't exist!");
            }

            if (IsAbleToCancel(request))
            {
                await NotifyApproversOnDecision(TimeOffRequestStatus.Cancelled, request);

                request.Requester.DaysOff += request.WorkingDaysOff;

                var approvers = request.Approvers.ToList();

                approvers.ForEach(approver => approver.TimeOffRequestsToApprove.Remove(request));

                request.Status = TimeOffRequestStatus.Cancelled;
                await _timeOffRequestRepository.SaveChanges();
            }
            else
            {
                throw new CannotCancelTimeOffRequestException($"TimeOffRequest with id: {id} cannot be cancelled!");
            }
        }

        private bool IsAbleToCancel(TimeOffRequest request)
        {
            bool canCancel = false;
            if ((request.Status == TimeOffRequestStatus.Created ||
                request.Status == TimeOffRequestStatus.Awaiting) &&
                (request.StartDate >= DateTime.Now.AddDays(3)))
            {
                canCancel = true;
            }
            return canCancel;
        }
    }
}