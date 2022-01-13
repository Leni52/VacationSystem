using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkForceManagement.DAL.Entities;

namespace WorkForceManagement.BLL.Services
{
    public interface ITimeOffRequestService
    {
        Task CreateTimeOffRequest(TimeOffRequest timeOffRequest, User currentUser);
        Task<List<TimeOffRequest>> GetAllRequests();
        Task<TimeOffRequest> GetTimeOffRequest(Guid Id);
        Task DeleteTimeOffRequest(Guid Id);
        Task<TimeOffRequest> UpdateTimeOffRequest(Guid timeOffRequestId, TimeOffRequest request);
        Task RejectTimeOffRequest(Guid timeOffRequestId, User currentUser);
        Task<string> CheckTimeOffRequest(Guid timeOffRequestId);
    }
}
