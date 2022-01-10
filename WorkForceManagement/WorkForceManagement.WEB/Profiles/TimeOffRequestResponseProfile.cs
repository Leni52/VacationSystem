using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkForceManagement.DAL.Entities;
using WorkForceManagement.DTO.Responses;

namespace WorkForceManagement.WEB.Profiles
{
    public class TimeOffRequestResponseProfile:Profile
    {
        public TimeOffRequestResponseProfile()
        {
            this.CreateMap<TimeOffRequest, TimeOffRequestResponseModel>();
        }
    }
}
