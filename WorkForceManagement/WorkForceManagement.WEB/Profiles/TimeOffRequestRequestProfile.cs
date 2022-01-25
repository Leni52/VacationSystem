using AutoMapper;
using WorkForceManagement.DAL.Entities;
using WorkForceManagement.DTO.Requests;

namespace WorkForceManagement.WEB.Profiles
{
    public class TimeOffRequestRequestProfile : Profile
    {
        public TimeOffRequestRequestProfile()
        {
            this.CreateMap<TimeOffRequestRequestDTO, TimeOffRequest>();
        }
    }
}