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
       
        public async Task CreateTimeOffRequest(TimeOffRequest timeOffRequest)
        {
            timeOffRequest.Status = 0;           
            await _timeOffRequestRepository.CreateOrUpdate(timeOffRequest);
        }    

        public async Task  DeleteTimeOffRequest(Guid Id)
        {
            var request =await _timeOffRequestRepository.Get(Id);
            if (request != null)
            {
              await _timeOffRequestRepository.Remove(request);
            }
            throw new ItemDoesNotExistException();
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
        public async Task UpdateTimeOffRequest(Guid Id, TimeOffRequestType timeOffRequestType)
        {
            TimeOffRequest requestToUpdate =await _timeOffRequestRepository.Get(Id);
            if (requestToUpdate == null)
            {
                throw new ItemDoesNotExistException();
            }
            requestToUpdate.Type = timeOffRequestType;

           await _timeOffRequestRepository.CreateOrUpdate(requestToUpdate);
        }     
               
    }
}
