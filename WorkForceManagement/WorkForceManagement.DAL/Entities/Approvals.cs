namespace WorkForceManagement.DAL.Entities
{
    public class Approvals
    {
        public Team Team { get; set; }
        public TimeOffRequest TimeOffRequest { get; set; }
        public ApprovalState State { get; set; }
    }
}
