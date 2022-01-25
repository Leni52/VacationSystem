using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkForceManagement.DAL.Entities;

namespace WorkForceManagement.BLL.Services.Interfaces
{
    public interface ITeamService
    {
        Task AddUserToTeam(Guid id, User user, User currentUser);
        Task Create(Team team, User currentUser);
        Task DeleteTeam(Guid id);
        Task<List<User>> GetAllTeamMembers(Guid id);
        Task<List<Team>> GetAllTeams();
        Task<Team> GetTeamWithId(Guid id);
        Task RemoveUserFromTeam(Guid id, User user, User currentUser);
        Task UpdateTeam(Team team, Guid teamId, User currentUser);
        Task UpdateTeamLeader(Guid id, User user, User currentUser);
    }
}