using System;

namespace WorkForceManagement.DTO.ResponseDTO
{
    public class TeamResponseDTO
    {
        public Guid Id { get; set; }
        public Guid TeamLeaderId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public DateTime CreationDate { get; set; }
        public string CreatorId { get; set; }
        public DateTime ChangeDate { get; set; }
        public string UpdaterId { get; set; }
    }
}
