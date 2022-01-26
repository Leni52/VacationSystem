using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkForceManagement.DAL.Entities;

namespace WorkForceManagement.BLL.Services.Interfaces
{
    public interface ITimeOffRequestService
    {
        Task CreateTimeOffRequest(TimeOffRequest request, User currentUser);
        Task<List<TimeOffRequest>> GetAllRequests();
        Task<TimeOffRequest> GetTimeOffRequest(Guid Id);
        Task DeleteTimeOffRequest(Guid Id);
        Task<TimeOffRequest> UpdateTimeOffRequest(Guid timeOffRequestId, TimeOffRequest request, string currentUserId);
        Task<List<TimeOffRequest>> GetMyRequests(Guid userId);
        Task<string> CheckTimeOffRequest(Guid id);
        Task AnswerTimeOffRequest(Guid id, bool isApproved, User currentUser, string reason);
        Task<List<User>> GetMyColleguesTimeOffRequests(User currentUser);
        Task CancelTimeOffRequest(Guid id);
        Task SaveFile(TblFile file, Guid TimeOffRequestId);
        Task<TblFile> GetFile(Guid TimeOffRequestId);
    }
}