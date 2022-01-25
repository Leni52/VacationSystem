using AutoMapper;
using WorkForceManagement.DAL.Entities;
using WorkForceManagement.DTO.ResponseDTO;

namespace WorkForceManagement.WEB.Profiles
{
    public class TeamResponseProfile : Profile
    {
        public TeamResponseProfile()
        {
            this.CreateMap<Team, TeamResponseDTO>();
        }
    }
}