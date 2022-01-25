using AutoMapper;
using WorkForceManagement.DAL.Entities;
using WorkForceManagement.DTO.Responses;

namespace WorkForceManagement.WEB.Profiles
{
    public class TimeOffRequestResponseProfile : Profile
    {
        public TimeOffRequestResponseProfile()
        {
            this.CreateMap<TimeOffRequest, TimeOffRequestResponseDTO>()
                .ForMember(response => response.StartDate, m => m.MapFrom(u => u.StartDate.ToString("dd/MM/yyyy")))
                 .ForMember(response => response.EndDate, m => m.MapFrom(u => u.EndDate.ToString("dd/MM/yyyy")));
        }
    }
}