using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public void CreateTimeOffRequestAsync(TimeOffRequest timeOffRequest)
        {
            _timeOffRequestRepository.CreateOrUpdate(timeOffRequest);
        }    

        public void  DeleteTimeOffRequest(Guid Id)
        {
            var request = _timeOffRequestRepository.Get(Id);
            if (request != null)
            {
               _timeOffRequestRepository.Remove(request);
            }
            throw new ItemDoesNotExistException();
        }
        public List<TimeOffRequest> GetAllRequests()
        {
            return _timeOffRequestRepository.All();
        }
        public TimeOffRequest GetTimeOffRequest(Guid Id)
        {
            TimeOffRequest timeOffRequest= _timeOffRequestRepository.Get(Id);
            if (timeOffRequest == null)
            {
                throw new ItemDoesNotExistException();
            }
            return timeOffRequest;
        }
        public void UpdateTimeOffRequest(Guid Id, TimeOffRequestType timeOffRequestType, 
            TimeOffRequestStatus timeOffRequestStatus, string description)
        {
            TimeOffRequest requestToUpdate = _timeOffRequestRepository.Get(Id);
            if (requestToUpdate == null)
            {
                throw new ItemDoesNotExistException();
            }
            requestToUpdate.Status = timeOffRequestStatus;
            requestToUpdate.Type = timeOffRequestType;
            requestToUpdate.Description = description;
            _timeOffRequestRepository.CreateOrUpdate(requestToUpdate);
        }       
    }
}
