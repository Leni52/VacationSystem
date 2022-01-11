using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkForceManagement.DAL.Entities;
using WorkForceManagement.DTO.ResponseModels;

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
