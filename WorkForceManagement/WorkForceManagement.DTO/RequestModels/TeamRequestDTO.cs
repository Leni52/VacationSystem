using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkForceManagement.DTO.RequestModels
{
    public class TeamRequestDTO
    {
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string TeamLeaderId { get; set; }
        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string Description { get; set; }
    }
}
