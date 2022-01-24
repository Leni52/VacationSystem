﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkForceManagement.DTO.Responses
{
   public class TimeOffRequestResponseDTO
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }        
    }
}
