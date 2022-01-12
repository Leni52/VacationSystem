using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkForceManagement.DTO.ResponseDTO
{
    public class TeamResponseDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid TeamLeaderId { get; set; }
        public string Description { get; set; }
    }
}
