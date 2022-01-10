using System;
using System.Collections.Generic;
using WorkForceManagement.DAL.Entities;

namespace WorkForceManagement.BLL.Services
{
    public interface ITimeOffRequestService
    {
        void CreateTimeOffRequestAsync(TimeOffRequest timeOffRequest);
        List<TimeOffRequest> GetAllRequests();
        TimeOffRequest GetTimeOffRequest(Guid Id);
        void DeleteTimeOffRequest(Guid Id);
        void UpdateTimeOffRequest(Guid Id, TimeOffRequestType timeOffRequestType,
            TimeOffRequestStatus timeOffRequestStatus, string description);


    }
}
