using System;

namespace WorkForceManagement.DTO.ResponseDTO
{
    public class TeamResponseDTO
    {
        public string Name { get; set; }
        public Guid TeamLeaderId { get; set; }
        public string Description { get; set; }
    }
}
