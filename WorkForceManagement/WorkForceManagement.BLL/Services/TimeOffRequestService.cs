using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkForceManagement.BLL.Exceptions;
using WorkForceManagement.DAL.Entities;
using WorkForceManagement.DAL.Repositories;

namespace WorkForceManagement.BLL.Services
{
    public class TimeOffRequestService : ITimeOffRequestService
    {
        private readonly IRepository<TimeOffRequest> _timeOffRequestRepository;
        public TimeOffRequestService(IRepository<TimeOffRequest> timeOffRequestRepository)
        {
            _timeOffRequestRepository = timeOffRequestRepository;
        }
       
        public async Task CreateTimeOffRequest(TimeOffRequest timeOffRequest, User currentUser)
        {
            timeOffRequest.Status = 0;
            timeOffRequest.CreatorId = currentUser.Id;
            List<User> approvers = currentUser.Teams.Select(team => team.TeamLeader).ToList();
            approvers.ForEach(user => timeOffRequest.Approvers.Add(user)); // gets all the approvers and adds them to timeOff

            await _timeOffRequestRepository.CreateOrUpdate(timeOffRequest);
        }    

        public async Task  DeleteTimeOffRequest(Guid Id)
        {
            var request =await _timeOffRequestRepository.Get(Id);
            if (request == null)
            {
                throw new ItemDoesNotExistException();
            }
            await _timeOffRequestRepository.Remove(request);            
        }
        public async Task<List<TimeOffRequest>> GetAllRequests()
        {
            return await _timeOffRequestRepository.All();
        }       

        public async Task<TimeOffRequest> GetTimeOffRequest(Guid Id)
        {
            TimeOffRequest timeOffRequest=await _timeOffRequestRepository.Get(Id);
            if (timeOffRequest == null)
            {
                throw new ItemDoesNotExistException();
            }
            return timeOffRequest;
        }
        public async Task<TimeOffRequest> UpdateTimeOffRequest(Guid timeOffRequestId, TimeOffRequest timeOffRequest, string currentUserId)
            
        {
            TimeOffRequest requestToUpdate =await _timeOffRequestRepository.Get(timeOffRequestId);
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
        public List<TimeOffRequest> GetMyRequests(string currentUserId)
        {
            List<TimeOffRequest> allRequests = _timeOffRequestRepository
              .Find(u => u.CreatorId == currentUserId);
            return allRequests;
        }

        public async Task ApproveTimeOffRequest(Guid requestId, User currentUser)
        {
            TimeOffRequest timeOffRequest = await _timeOffRequestRepository.Get(requestId);
           
            timeOffRequest.Status = TimeOffRequestStatus.Approved;
            timeOffRequest.ChangeDate = DateTime.Now;
            timeOffRequest.UpdaterId = currentUser.Id;
            timeOffRequest.AlreadyApproved.Add(currentUser);
            await _timeOffRequestRepository.SaveChanges();

        }
    }
}
