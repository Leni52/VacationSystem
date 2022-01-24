using System;
using System.ComponentModel.DataAnnotations;
using WorkForceManagement.DAL.Entities;
using WorkForceManagement.DTO.Constants;

namespace WorkForceManagement.DTO.Requests
{
    public class TimeOffRequestRequestDTO
    {
        [Required(ErrorMessage = Constants.Constants.ErrorDescription)]
        [MinLength(10, ErrorMessage = Constants.Constants.ErrorDescriptionMin)]
        [MaxLength(50)]
        public string Description { get; set; }
        [Required(ErrorMessage = Constants.Constants.ErrorTimeOffRequestType)]
        public TimeOffRequestType Type { get; set; }
        [Required(ErrorMessage = Constants.Constants.ErrorStartDate)]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = Constants.Constants.ErrorEndDate)]
        public DateTime EndDate { get; set; }
       
    }
}
