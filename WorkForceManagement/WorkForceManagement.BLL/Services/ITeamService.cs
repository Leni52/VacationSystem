using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkForceManagement.DAL.Entities;

namespace WorkForceManagement.BLL.Services
{
    public interface ITeamService
    {
        Task AddUserToTeam(Guid teamId, User user, User currentUser);
        Task Create(Team teamToAdd, User currentUser);
        Task DeleteTeam(Guid teamId);
        Task<List<User>> GetAllTeamMembers(Guid teamId);
        Task<List<Team>> GetAllTeams();
        Task<Team> GetTeamWithId(Guid teamId);
        Task RemoveUserFromTeam(Guid teamId, User user, User currentUser);
        Task UpdateTeam(Team teamToUpdate, Guid teamId, User currentUser);
        Task UpdateTeamLeader(Guid teamId, User user, User currentUser);
       
    }
}