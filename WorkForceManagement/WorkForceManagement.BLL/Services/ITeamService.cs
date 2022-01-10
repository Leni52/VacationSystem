using System.Collections.Generic;
using System.Threading.Tasks;
using WorkForceManagement.DAL.Entities;

namespace WorkForceManagement.BLL.Services
{
    public interface ITeamService
    {
        Task CreateAsync(Team teamToAdd);
        Task<Team> GetTeamWithIdAsync(string teamId);
        Task<List<Team>> GetAllTeamsAsync();
        Task UpdateTeamAsync(Team updatedTeam, string teamId);
        Task DeleteTeamAsync(string teamToDeleteId);
        Task UpdateTeamLeaderAsync(string teamId, User newTeamLeader);
        Task AddUserToTeamAsync(string teamId, User userToAdd);
        Task<List<User>> GetAllTeamMembers(string teamId);
        Task RemoveUserFromTeam(string teamId, User userToDelete);
    }
}