using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkForceManagement.DTO.Requests
{
   public class TimeOffRequestModel
    {
        [Required(ErrorMessage ="Please, add a description:")]
        [MinLength(10, ErrorMessage ="The description is too short.")]
        [MaxLength(50)]
        public string Description { get; set; }
        [Required(ErrorMessage ="Please, add the type for your request:")]
        public string TimeOffRequestType { get; set; }
        [Required(ErrorMessage ="Please, add a start date: ")]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "Please, add a end date: ")]
        public DateTime EndDate { get; set; }
        //add validation for startdate>enddate
    }
}
