using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkForceManagement.DAL.Entities;

namespace WorkForceManagement.BLL.Services
{
    public interface ITeamService
    {
        Task AddUserToTeam(Guid teamId, User userToAdd);
        Task Create(Team teamToAdd);
        Task DeleteTeam(Guid teamToDeleteId);
        Task<List<User>> GetAllTeamMembers(Guid teamId);
        Task<List<Team>> GetAllTeams();
        Task<Team> GetTeamWithId(Guid teamId);
        Task RemoveUserFromTeam(Guid teamId, User userToDelete);
        Task UpdateTeam(Team updatedTeam, Guid teamId);
        Task UpdateTeamLeader(Guid teamId, User newTeamLeader);
    }
}