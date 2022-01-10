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

    }
}