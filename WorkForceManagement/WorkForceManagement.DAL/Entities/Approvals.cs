namespace WorkForceManagement.DAL.Entities
{
    public class Approvals : Base
    {
        public virtual Team Team { get; set; }
        public virtual TimeOffRequest TimeOffRequest { get; set; }
        public virtual ApprovalState State { get; set; }
    }
}
