using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkForceManagement.DTO.Responses
{
   public class TimeOffRequestResponseModel
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string  RequesterId{ get; set; }
    }
}
