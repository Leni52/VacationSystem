﻿using System;
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
        private readonly IMailService _mailService;

        public TimeOffRequestService(IRepository<TimeOffRequest> timeOffRequestRepository, IMailService mailService)
        {
            _timeOffRequestRepository = timeOffRequestRepository;
            _mailService = mailService;
        }

        public async Task CreateTimeOffRequest(TimeOffRequest timeOffRequest, User currentUser)
        {

            timeOffRequest.Status = 0;
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
                bool teamLeaderIsAway = false;
                var requests = user.CreatedTimeOffRequests.Where(x => x.Status == TimeOffRequestStatus.Approved && x.StartDate <= DateTime.Now
                && x.EndDate >= DateTime.Now).Any();
                if (requests)
                { // if the leader has an approved tof in the time of creating the request dont make him approver
                    teamLeaderIsAway = true;
                    break;
                }
                if (!teamLeaderIsAway)
                    validatedApprovers.Add(user);
            }
            return validatedApprovers;
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
            TimeOffRequest requestToUpdate = await _timeOffRequestRepository.Get(timeOffRequestId);
            if (requestToUpdate == null)
            {
                throw new ItemDoesNotExistException();
            }
            requestToUpdate.ChangeDate = DateTime.Now;
            requestToUpdate.UpdaterId = currentUserId;
            requestToUpdate.Type = timeOffRequest.Type;
            requestToUpdate.Description = timeOffRequest.Description;
            requestToUpdate.StartDate = timeOffRequest.StartDate;
            requestToUpdate.EndDate = timeOffRequest.EndDate;

            await _timeOffRequestRepository.CreateOrUpdate(requestToUpdate);
            return requestToUpdate;
        }
        public async Task<List<TimeOffRequest>> GetMyRequests(string currentUserId)
        {
            List<TimeOffRequest> allRequests = await _timeOffRequestRepository.All();
            return allRequests.Where(u => u.CreatorId == currentUserId).ToList();
        }

        public async Task AnswerTimeOffRequest(Guid timeOffRequestId, bool isApproved, User currentUser)
        {
            TimeOffRequest timeOffRequest = await GetTimeOffRequest(timeOffRequestId);

            if (timeOffRequest.Status == TimeOffRequestStatus.Approved ||
                timeOffRequest.Status == TimeOffRequestStatus.Rejected)
            { // the time off request has already been decided
                throw new TimeOffRequestIsClosedException($"Time off request with id:{timeOffRequestId}, is already closed");
            }

            bool currentUserIsApprover = timeOffRequest.Approvers.Any(user => user.Id == currentUser.Id);
            if (currentUserIsApprover == false)
                throw new UserIsntApproverException($"User with Id:{currentUser.Id}, cant approve this Time Off Request");

            timeOffRequest.ChangeDate = DateTime.Now;
            timeOffRequest.UpdaterId = currentUser.Id;

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

            // removes the request from the approvers timeOffRequestsToApprove
            timeOffRequest.Approvers.ForEach(approver => approver.TimeOffRequestsToApprove.Remove(timeOffRequest));

            await _timeOffRequestRepository.SaveChanges();
            //TODO send email to teamLeaders 

            //email to requester            
            await SendMailToRequesterApprovedRequest(timeOffRequest.Id, currentUser);
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

            int numberOfApprovers = timeOffRequest.Approvers.ToList().Count; // get all needed approvals

            if (numberOfApprovers == timeOffRequest.AlreadyApproved.ToList().Count ||
                timeOffRequest.Type == TimeOffRequestType.SickLeave) // compare with current approvals OR it's sick leave
            {
                timeOffRequest.Status = TimeOffRequestStatus.Approved;
              await  SendMailToRequesterApprovedRequest(timeOffRequest.Id, timeOffRequest.Requester);
                // To all approvers, removing the request from toApprove and adding it to Approved list
                List<User> approvers = timeOffRequest.Approvers.ToList();

                approvers.ForEach(approver => approver.TimeOffRequestsToApprove.Remove(timeOffRequest));
                approvers.ForEach(approver => approver.TimeOffRequestsApproved.Add(timeOffRequest));

                await _timeOffRequestRepository.SaveChanges();
                return "Approved";
            }
            else
            {
                timeOffRequest.Status = TimeOffRequestStatus.Awaiting; // in case it has a Created status
                await _timeOffRequestRepository.SaveChanges();
                return "Awaiting";
            }
        }
        public async Task SendMailToRequesterApprovedRequest(Guid requestId, User user)
        {
            TimeOffRequest request = await _timeOffRequestRepository.Get(requestId);
            //email to requester
            MailRequest mailRequest = new MailRequest();
            mailRequest.Body = $"Your request between {request.StartDate}" +
                $" and {request.EndDate}  has been approved.";
            mailRequest.Subject = "Approved request.";
            mailRequest.ToEmail = request.Requester.Email;
            await _mailService.SendEmail(mailRequest);
        }
        public async Task SendMailToRequesterApprovedRejected(Guid requestId, User user)
        {
            TimeOffRequest request = await _timeOffRequestRepository.Get(requestId);
            //email to requester            
            MailRequest mailRequest = new MailRequest();
            mailRequest.Body = $"Your request with start date: {request.StartDate} " +
                $" and end date: {request.EndDate} has been rejected.";
            mailRequest.Subject = "Rejected request.";
            mailRequest.ToEmail = request.Requester.Email;
            await _mailService.SendEmail(mailRequest);
        }        
    }
}
