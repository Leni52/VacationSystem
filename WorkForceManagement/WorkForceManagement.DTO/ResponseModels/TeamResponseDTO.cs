﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkForceManagement.DTO.ResponseModels
{
    public class TeamResponseDTO
    {
        public string Name { get; set; }
        public Guid TeamLeaderId { get; set; }
        public string Description { get; set; }
    }
}
