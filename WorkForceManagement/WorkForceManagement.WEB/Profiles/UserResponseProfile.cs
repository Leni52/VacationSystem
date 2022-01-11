using AutoMapper;
using WorkForceManagement.DAL.Entities;
using WorkForceManagement.DTO.ResponseDTO;

namespace WorkForceManagement.WEB.Profiles
{
    public class UserResponseProfile : Profile
    {
        public UserResponseProfile()
        {
            this.CreateMap<User, UserResponseDTO>();
        }
    }
}
