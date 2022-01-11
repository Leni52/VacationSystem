using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkForceManagement.DAL.Entities;
using WorkForceManagement.DTO.RequestModels;

namespace WorkForceManagement.WEB.Profiles
{
    public class TeamRequestProfile : Profile
    {
        public TeamRequestProfile()
        {
            this.CreateMap<TeamRequestDTO, Team>();
        }
    }
}
