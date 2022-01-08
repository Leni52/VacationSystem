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
        public Task<bool> CreateTimeOffRequestAsync()
        {
            throw new NotImplementedException();
        }

        public TimeOffRequest DeleteTimeOffRequest(Guid Id)
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
            return _timeOffRequestRepository.Get(Id);
        }

        public TimeOffRequest UpdateTimeOffRequest(Guid Id, TimeOffRequest timeOffRequest)
        {
            throw new NotImplementedException();
        }
    }
}
