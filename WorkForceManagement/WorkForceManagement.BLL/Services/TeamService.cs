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
            _teamRepository = teamRepository;
        }

        public async Task Create(Team teamToAdd)
        {
            Team teamWithSameName = _teamRepository.Get(team => team.Name == teamToAdd.Name);
            if (teamWithSameName != null)
                throw new TeamWithSameNameExistsException($"Team with the name:{teamToAdd.Name} already exists!");


            teamToAdd.CreationDate = DateTime.Now;
            teamToAdd.ChangeDate = DateTime.Now;
            teamToAdd.CreatorId = Guid.NewGuid().ToString("D"); // TODO Change to currentUser
            teamToAdd.UpdaterId = Guid.NewGuid().ToString("D"); // TODO Change to currentUser

            _teamRepository.CreateOrUpdate(teamToAdd);
        }
        public async Task<Team> GetTeamWithId(Guid teamId)
        {
            Team foundTeam = _teamRepository.Get(teamId);
            if (foundTeam == null)
                throw new KeyNotFoundException($"Team with id:{teamId} does not exist!");

            return foundTeam;
        }

        public async Task<List<Team>> GetAllTeams()
        {
            return _teamRepository.All();
        }

        public async Task UpdateTeam(Team updatedTeam, Guid teamId)
        {
            Team teamWithSameName = _teamRepository.Get(
                team => 
                team.Name == updatedTeam.Name &&
                team.Id != teamId); // find different team with same name

            if (teamWithSameName != null)
                throw new TeamWithSameNameExistsException($"Team with the name:{updatedTeam.Name} already exists!");

            Team foundTeam = await GetTeamWithId(teamId);


            foundTeam.ChangeDate = DateTime.Now;
            foundTeam.Name = updatedTeam.Name;
            foundTeam.Description = updatedTeam.Description;

            // foundTeam.UpdaterId = currentUser; //TODO

            _teamRepository.CreateOrUpdate(foundTeam);
        }
        public async Task DeleteTeam(Guid teamToDeleteId)
        {
            Team teamToDelete = await GetTeamWithId(teamToDeleteId);

            _teamRepository.Remove(teamToDelete);
        }
        public async Task UpdateTeamLeader(Guid teamId, User newTeamLeader)
        {
            Team foundTeam = await GetTeamWithId(teamId);

            //foundTeam.UpdaterId = currentUser; // TODO

            foundTeam.TeamLeader = newTeamLeader;

            _teamRepository.CreateOrUpdate(foundTeam);
        }

        public async Task AddUserToTeam(Guid teamId, User userToAdd)
        {
            Team foundTeam = await GetTeamWithId(teamId);

            foundTeam.Members.Add(userToAdd);

            _teamRepository.SaveChanges();
        }
        public async Task<List<User>> GetAllTeamMembers(Guid teamId)
        {
            Team foundTeam = await GetTeamWithId(teamId);

            return foundTeam.Members;
        }

        public async Task RemoveUserFromTeam(Guid teamId, User userToDelete)
        {
            Team foundTeam = await GetTeamWithId(teamId);

            if (foundTeam.Members.Any(user => user.Id == userToDelete.Id) == false) // the user to remove isnt part of the team
                throw new KeyNotFoundException($" User with id:{userToDelete.Id} isnt part of this team!");

            foundTeam.Members.Remove(userToDelete);

            _teamRepository.SaveChanges();
        }


    }
}
