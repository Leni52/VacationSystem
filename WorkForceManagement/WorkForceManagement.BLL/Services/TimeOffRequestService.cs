using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkForceManagement.BLL.Exceptions;
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

        public async Task CreateTimeOffRequest(TimeOffRequest timeOffRequest, User currentUser)
        {
            ValidateTimeOffRequestDates(timeOffRequest.StartDate, timeOffRequest.EndDate, currentUser);
            timeOffRequest.WorkingDaysOff = ValidateDaysOff(timeOffRequest.StartDate, timeOffRequest.EndDate);

            timeOffRequest.Status = TimeOffRequestStatus.Created;
            timeOffRequest.CreatorId = currentUser.Id;
            timeOffRequest.UpdaterId = currentUser.Id;
            List<User> approvers = GetApprovers(currentUser);

            approvers.ForEach(user => timeOffRequest.Approvers.Add(user));
            approvers.ForEach(user => user.TimeOffRequestsToApprove.Add(timeOffRequest));
            currentUser.CreatedTimeOffRequests.Add(timeOffRequest);

            await _timeOffRequestRepository.CreateOrUpdate(timeOffRequest);
            await CheckTimeOffRequest(timeOffRequest.Id);
        }

        private List<User> GetApprovers(User currentUser)
        { // gets all the approvers and checks if they themselves are not in a timeoff
            List<User> potentialApprovers = currentUser.Teams.Select(team => team.TeamLeader).ToList();
            List<User> validatedApprovers = new List<User>();

            foreach (User user in potentialApprovers)
            {
                bool teamLeaderIsAway = user.CreatedTimeOffRequests.Any(x => x.Status == TimeOffRequestStatus.Approved && x.StartDate.Date <= DateTime.Now.Date
                && x.EndDate.Date >= DateTime.Now.Date);

                if (!teamLeaderIsAway) // if approver is not away today add him as validatedApprover
                    validatedApprovers.Add(user);
            }
            return validatedApprovers;
        }

        private void ValidateTimeOffRequestDates(DateTime startDate, DateTime endDate, User currentUser)
        {
            if (startDate > endDate)
                throw new InvalidDatesException("Invalid time off request dates, the start date should be earlier or equal to end date");

            int requestedDays = ValidateDaysOff(startDate, endDate);
            int totalDays = currentUser.DaysOff;
            if (requestedDays > totalDays)
                throw new InvalidDatesException("The number of requested days exceeds the maximum number available");

            bool isOverlapping = currentUser.CreatedTimeOffRequests.Any(
                timeOff =>
                timeOff.Status != TimeOffRequestStatus.Rejected &&
                timeOff.Status != TimeOffRequestStatus.Cancelled &&
                timeOff.StartDate < endDate &&
                startDate < timeOff.EndDate
                );

            if (isOverlapping) // if current user already has a timeoffrequest thats not rejected and is overlapping with the current timeOffRequest
                throw new OverlappingTimeOffRequestsException($"User with id: {currentUser.Id}, already has a time off request thats overlapping with the current request!");


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
            TimeOffRequest timeOffRequest = await _timeOffRequestRepository.Get(Id);
            if (timeOffRequest == null)
            {
                throw new ItemDoesNotExistException();
            }
            return timeOffRequest;
        }

        public async Task<TimeOffRequest> UpdateTimeOffRequest(Guid timeOffRequestId, TimeOffRequest timeOffRequest, string currentUserId)

        {   
            ValidateTimeOffRequestDates(timeOffRequest.StartDate, timeOffRequest.EndDate, await _userService.GetUserById(Guid.Parse(currentUserId)));
            int daysOff =  ValidateDaysOff(timeOffRequest.StartDate, timeOffRequest.EndDate);

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

        public async Task<List<TimeOffRequest>> GetMyRequests(string currentUserId)
        {
            List<TimeOffRequest> allRequests = await _timeOffRequestRepository.All();
            return allRequests.Where(u => u.CreatorId == currentUserId).ToList();
        }

        public async Task AnswerTimeOffRequest(Guid timeOffRequestId, bool isApproved, User currentUser, string reason)
        {
            TimeOffRequest timeOffRequest = await GetTimeOffRequest(timeOffRequestId);

            if (timeOffRequest.Status == TimeOffRequestStatus.Approved ||
                timeOffRequest.Status == TimeOffRequestStatus.Rejected ||
                timeOffRequest.Status == TimeOffRequestStatus.Cancelled)
            { // the time off request has already been decided
                throw new TimeOffRequestIsClosedException($"Time off request with id:{timeOffRequestId}, is already closed");
            }

            bool currentUserIsApprover = timeOffRequest.Approvers.Any(user => user.Id == currentUser.Id);
            if (currentUserIsApprover == false)
                throw new UserIsntApproverException($"User with Id:{currentUser.Id}, cant approve this Time Off Request");

            timeOffRequest.ChangeDate = DateTime.Now.Date;
            timeOffRequest.UpdaterId = currentUser.Id;
            if(reason.Length != 0)
            {
                timeOffRequest.Reason = reason;
            }

            if (timeOffRequest.Status == TimeOffRequestStatus.Created)
            {
                timeOffRequest.Status = TimeOffRequestStatus.Awaiting;
            }

            if (isApproved)
            {
                await ApproveTimeOffRequest(timeOffRequest, currentUser);
            }
            else
            {
                await RejectTimeOffRequest(timeOffRequest, currentUser);
            }
        }

        public async Task RejectTimeOffRequest(TimeOffRequest timeOffRequest, User currentUser)
        {
            timeOffRequest.Status = TimeOffRequestStatus.Rejected;

            await SendMailToRequesterRejectedRequest(timeOffRequest.Id, currentUser);
            await NotifyApproversOnDecision(TimeOffRequestStatus.Rejected, timeOffRequest);

            // removes the request from the approvers timeOffRequestsToApprove
            timeOffRequest.Approvers.ForEach(approver => approver.TimeOffRequestsToApprove.Remove(timeOffRequest));

            await _timeOffRequestRepository.SaveChanges();
        }

        public async Task ApproveTimeOffRequest(TimeOffRequest timeOffRequest, User currentUser)
        {
            timeOffRequest.AlreadyApproved.Add(currentUser);
            await _timeOffRequestRepository.SaveChanges();
            //email to requester
            await SendMailToRequesterApprovedRequest(timeOffRequest.Id, currentUser);
            await CheckTimeOffRequest(timeOffRequest.Id);
        }

        public async Task<string> CheckTimeOffRequest(Guid timeOffRequestId)
        {
            TimeOffRequest timeOffRequest = await GetTimeOffRequest(timeOffRequestId);

            if (timeOffRequest == null)
            {
                throw new ItemDoesNotExistException();
            }
            if (timeOffRequest.Status == TimeOffRequestStatus.Rejected)
            {
                return "Rejected";
            }
            else if (timeOffRequest.Status == TimeOffRequestStatus.Approved)
            {
                return "Approved";
            }
            else if(timeOffRequest.Status == TimeOffRequestStatus.Cancelled)
            {
                return "Cancelled";
            }

            int numberOfApprovers = timeOffRequest.Approvers.ToList().Count; // get all needed approvals

            if (numberOfApprovers == timeOffRequest.AlreadyApproved.ToList().Count ||
                timeOffRequest.Type == TimeOffRequestType.SickLeave) // compare with current approvals OR it's sick leave
            {
                timeOffRequest.Status = TimeOffRequestStatus.Approved;
                await NotifyApproversOnDecision(TimeOffRequestStatus.Approved, timeOffRequest);
                await SendMailToRequesterApprovedRequest(timeOffRequest.Id, timeOffRequest.Requester);
                // To all approvers, removing the request from toApprove and adding it to Approved list
                List<User> approvers = timeOffRequest.Approvers.ToList();

                approvers.ForEach(approver => approver.TimeOffRequestsToApprove.Remove(timeOffRequest));
                approvers.ForEach(approver => approver.TimeOffRequestsApproved.Add(timeOffRequest));

                timeOffRequest.Requester.DaysOff -= ValidateDaysOff(timeOffRequest.StartDate, timeOffRequest.EndDate);
                //subtract the available days since the request is approved
                await _timeOffRequestRepository.SaveChanges();

                await NotifyTeamMembersLeaderIsOOO(timeOffRequest);
                return "Approved";
            }
            else
            {// sending notifications to leaders which nave not yet confirmed, in case of call after creaton it's sent to all team leaders
                List<User> approversToSendEmailTo = timeOffRequest.Approvers.Except(timeOffRequest.AlreadyApproved).ToList();

                foreach (User u in approversToSendEmailTo)
                {
                    await _mailService.SendEmail(new MailRequest()
                    {
                        ToEmail = u.Email,
                        Body = $"[TimeOffRequest] {timeOffRequest.CreatorId}",
                        Subject = $"User with id {timeOffRequest.CreatorId} is requesting a TOR between the dates {timeOffRequest.StartDate.ToShortDateString()} and {timeOffRequest.EndDate.ToShortDateString()}!"
                    });
                }

                await _timeOffRequestRepository.SaveChanges();
                return timeOffRequest.Status == TimeOffRequestStatus.Awaiting ? "Awaiting" : "Created";
            }
        }

        public async Task SendMailToRequesterApprovedRequest(Guid requestId, User user)
        {
            TimeOffRequest request = await _timeOffRequestRepository.Get(requestId);
            //email to requester
            MailRequest mailRequest = new MailRequest();
            mailRequest.Body = $"Your request between {request.StartDate.ToShortDateString()}" +
                $" and {request.EndDate.ToShortDateString()}  has been approved.";
            mailRequest.Subject = "Approved request.";
            mailRequest.ToEmail = request.Requester.Email;
            await _mailService.SendEmail(mailRequest);
        }

        public async Task SendMailToRequesterRejectedRequest(Guid requestId, User user)
        {
            TimeOffRequest request = await _timeOffRequestRepository.Get(requestId);
            //email to requester
            MailRequest mailRequest = new MailRequest();
            mailRequest.Body = $"Your request with start date: {request.StartDate.ToShortDateString()} " +
                $" and end date: {request.EndDate.ToShortDateString()} has been rejected.";
            mailRequest.Subject = "Rejected request.";
            mailRequest.ToEmail = request.Requester.Email;
            await _mailService.SendEmail(mailRequest);
        }

        private async Task NotifyTeamMembersLeaderIsOOO(TimeOffRequest timeOffRequest)
        {
            List<User> usersToSendEmailTo = await _userService.GetUsersUnderTeamLeader(timeOffRequest.Requester);

            if (usersToSendEmailTo.Count != 0)
            {
                foreach (User u in usersToSendEmailTo)
                {
                    await _mailService.SendEmail(new MailRequest()
                    {
                        ToEmail = u.Email,
                        Subject = "TeamLeader OOO",
                        Body = $"{timeOffRequest.Requester.UserName} is OOO until {timeOffRequest.EndDate.ToShortDateString()}!"
                    });
                }
            }
        }

        private async Task NotifyApproversOnDecision(TimeOffRequestStatus status, TimeOffRequest timeOffRequest)
        {
            string subject = "";
            string body = "";
            if (status == TimeOffRequestStatus.Approved)
            {
                subject = "Time off Request Approved";
                body = $"Time off request by: {timeOffRequest.Requester.UserName} with start date: {timeOffRequest.StartDate.ToShortDateString()} and end date: {timeOffRequest.EndDate.ToShortDateString()} is APPROVED";
            }
            else if (status == TimeOffRequestStatus.Rejected)
            {
                subject = "Time off request Rejected";
                body = $"Time off request by: {timeOffRequest.Requester.UserName} with start date: {timeOffRequest.StartDate.ToShortDateString()} and end date: {timeOffRequest.EndDate.ToShortDateString()} is REJECTED";
            } else if (status == TimeOffRequestStatus.Cancelled)
            {
                subject = "Time off request Cancelled";
                body = $"Time off request by: {timeOffRequest.Requester.UserName} with start date: {timeOffRequest.StartDate.ToShortDateString()} and end date: {timeOffRequest.EndDate.ToShortDateString()} has been CANCELLED";
            }

            List<User> approvers = timeOffRequest.Approvers.ToList();

            foreach (User approver in approvers)
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
            List<Team> userTeams = _userService.GetUserTeams(currentUser);
            List<User> teamMembers = new List<User>();
            List<User> teamMembersOnVacation = new List<User>();

            foreach (var team in userTeams)
            {
                teamMembers = await _teamService.GetAllTeamMembers(team.Id);
            }

            foreach (var user in teamMembers)
            {
                if ((user.CreatedTimeOffRequests.Where(t => t.Status == TimeOffRequestStatus.Approved).Any()) &&                    
                    (user.CreatedTimeOffRequests.Where(t => t.EndDate >= DateTime.Now).Any()))
                {
                    teamMembersOnVacation.Add(user);
                }
            }

            return teamMembersOnVacation;
        }

        public async Task CancelTimeOffRequest(Guid timeOffRequestId)
        {
            TimeOffRequest timeOffRequest = await _timeOffRequestRepository.Get(timeOffRequestId);

            if (timeOffRequest == null)
            {
                throw new ItemDoesNotExistException($"TimeOffRequest with id: {timeOffRequestId} doesn't exist!");
            }

            if (IsAbleToCancel(timeOffRequest))
            {
                await NotifyApproversOnDecision(TimeOffRequestStatus.Cancelled, timeOffRequest);

                timeOffRequest.Status = TimeOffRequestStatus.Cancelled;
            }
            else
            {
                throw new CannotCancelTimeOffRequestException($"TimeOffRequest with id: {timeOffRequestId} cannot be cancelled!"); 
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