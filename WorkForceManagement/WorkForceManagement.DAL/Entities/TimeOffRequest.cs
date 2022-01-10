using System;

namespace WorkForceManagement.DAL.Entities
{
    public class TimeOffRequest : Base
    {
        public virtual User Requester { get; set; }
        public virtual TimeOffRequestType Type { get; set; }
        public virtual TimeOffRequestStatus Status { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

    }
}
