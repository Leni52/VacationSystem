using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkForceManagement.DAL.Entities;

namespace WorkForceManagement.BLL.Services
{
   public interface ITimeOffRequestService
    {
        public Task<bool> CreateTimeOffRequestAsync();
        public List<TimeOffRequest> GetAllRequests();
        public TimeOffRequest GetTimeOffRequest(Guid Id);
        public TimeOffRequest DeleteTimeOffRequest(Guid Id);
        public TimeOffRequest UpdateTimeOffRequest(Guid Id, TimeOffRequest timeOffRequest);


    }
}
