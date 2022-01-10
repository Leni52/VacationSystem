using AutoMapper;
using WorkForceManagement.DAL.Entities;
using WorkForceManagement.DTO.RequestModels;

namespace WorkForceManagement.WEB.Profiles
{
    public class UserRequestProfile : Profile
    {
        public UserRequestProfile()
        {
            this.CreateMap<UserRequestModel, User>();
        }
    }
}
