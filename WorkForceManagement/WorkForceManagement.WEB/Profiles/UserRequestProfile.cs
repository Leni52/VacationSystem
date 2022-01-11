using AutoMapper;
using WorkForceManagement.DAL.Entities;
using WorkForceManagement.DTO.RequestDTO;

namespace WorkForceManagement.WEB.Profiles
{
    public class UserRequestProfile : Profile
    {
        public UserRequestProfile()
        {
            this.CreateMap<UserRequestDTO, User>();
        }
    }
}
