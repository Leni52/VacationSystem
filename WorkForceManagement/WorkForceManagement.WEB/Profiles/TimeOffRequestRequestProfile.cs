using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkForceManagement.DAL.Entities;
using WorkForceManagement.DTO.Requests;

namespace WorkForceManagement.WEB.Profiles
{
    public class TimeOffRequestRequestProfile:Profile
    {
        public TimeOffRequestRequestProfile()
        {
            this.CreateMap<TimeOffRequestModel, TimeOffRequest>();
        }
    }
}
