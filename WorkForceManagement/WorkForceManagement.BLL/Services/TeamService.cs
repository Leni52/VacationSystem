using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkForceManagement.BLL.Exceptions;
using WorkForceManagement.DAL.Entities;
using WorkForceManagement.DAL.Repositories;

namespace WorkForceManagement.BLL.Services
{
    public class TeamService : ITeamService
    {
        private readonly IRepository<Team> _teamRepository;

        public TeamService(IRepository<Team> teamRepository)
        {
            this._teamRepository = teamRepository;
        }

        public async Task CreateAsync(Team teamToAdd)
        {
            Team teamWithSameName = _teamRepository.Get(team => team.Name == teamToAdd.Name);
            if (teamWithSameName != null)
                throw new TeamWithSameNameExistsException($"Team with the name:{teamToAdd.Name} already exists!");

            //teamToAdd.Id = Guid.NewGuid();
            teamToAdd.CreationDate = DateTime.Now;
            teamToAdd.ChangeDate = DateTime.Now;
            teamToAdd.CreatorId = Guid.NewGuid().ToString("D");
            teamToAdd.UpdaterId = Guid.NewGuid().ToString("D");

            _teamRepository.CreateOrUpdate(teamToAdd);
        }
        public async Task<Team> GetTeamWithIdAsync(string teamId)
        {
            Team foundTeam = _teamRepository.Get(Guid.Parse(teamId));
            if (foundTeam == null)
                throw new KeyNotFoundException($"Team with id:{teamId} does not exist!");

            return foundTeam;
        }

        public async Task<List<Team>> GetAllTeamsAsync()
        {
            return _teamRepository.All();
        }

        public async Task UpdateTeamAsync(Team updatedTeam, string teamId)
        {
            Team foundTeam = _teamRepository.Get(Guid.Parse(teamId));
            if (foundTeam == null)
                throw new KeyNotFoundException($"Team with id:{teamId} was not found!");

            foundTeam.ChangeDate = DateTime.Now;
            foundTeam.Name = updatedTeam.Name;
            foundTeam.Description = updatedTeam.Description;

            // foundTeam.UpdaterId = currentUser; //TODO

            _teamRepository.CreateOrUpdate(foundTeam);
        }
        public async Task DeleteTeamAsync(string teamToDeleteId)
        {
            Team teamToDelete = _teamRepository.Get(Guid.Parse(teamToDeleteId));
            if(teamToDelete == null)
            {
                throw new KeyNotFoundException($"Team with id:{teamToDeleteId} was not found!");
            }

            _teamRepository.Remove(teamToDelete);
        }
        public async Task UpdateTeamLeaderAsync(string teamId, User newTeamLeader)
        {
            Team foundTeam = _teamRepository.Get(Guid.Parse(teamId));
            if(foundTeam == null)
                throw new KeyNotFoundException($"Team with id:{teamId} was not found!");

            //foundTeam.UpdaterId = currentUser; // TODO

            foundTeam.TeamLeader = newTeamLeader;

            _teamRepository.CreateOrUpdate(foundTeam);
        }


    }
}
