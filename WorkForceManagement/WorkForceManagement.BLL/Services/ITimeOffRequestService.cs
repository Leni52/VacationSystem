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
        Task<TimeOffRequest> UpdateTimeOffRequest(Guid timeOffRequestId, TimeOffRequest request, string currentUserId);
        Task<List<TimeOffRequest>> GetMyRequests(string currentUserId);
        Task<string> CheckTimeOffRequest(Guid timeOffRequestId);
        Task AnswerTimeOffRequest(Guid timeOffRequestId, bool isApproved, User currentUser);
        Task<List<User>> GetMyColleguesTimeOffRequests(User currentUser);
    }
}