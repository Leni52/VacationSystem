using System;
using System.ComponentModel.DataAnnotations;
using WorkForceManagement.DAL.Entities;

namespace WorkForceManagement.DTO.Requests
{
    public class TimeOffRequestRequestModel
    {
        [Required(ErrorMessage = "Please, add a description:")]
        [MinLength(10, ErrorMessage = "The description is too short.")]
        [MaxLength(50)]
        public string Description { get; set; }
        [Required(ErrorMessage = "Please, add the type for your request:")]
        public int TimeOffRequestType { get; set; }
        [Required(ErrorMessage = "Please, add the type for your request:")]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "Please, add a end date: ")]
        public DateTime EndDate { get; set; }
        //add validation for startdate>enddate
    }
}
