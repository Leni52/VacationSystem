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
            Team teamWithSameName = await _teamRepository.Get(team => team.Name == teamToAdd.Name);
            if (teamWithSameName != null)
                throw new TeamWithSameNameExistsException($"Team with the name:{teamToAdd.Name} already exists!");


            teamToAdd.CreationDate = DateTime.Now;
            teamToAdd.ChangeDate = DateTime.Now;
            teamToAdd.CreatorId = Guid.NewGuid().ToString("D"); // TODO Change to currentUser
            teamToAdd.UpdaterId = Guid.NewGuid().ToString("D"); // TODO Change to currentUser

            await _teamRepository.CreateOrUpdate(teamToAdd);
        }
        public async Task<Team> GetTeamWithId(Guid teamId)
        {
            Team foundTeam = await _teamRepository.Get(teamId);
            if (foundTeam == null)
                throw new KeyNotFoundException($"Team with id:{teamId} does not exist!");

            return foundTeam;
        }
        public async Task<List<Team>> GetAllTeams()
        {
            return await _teamRepository.All();
        }
        public async Task UpdateTeam(Team teamToUpdate, Guid teamId)
        {
            Team teamWithSameName = await _teamRepository.Get(
                team => 
                team.Name == teamToUpdate.Name &&
                team.Id != teamId); // find different team with same name

            if (teamWithSameName != null)
                throw new TeamWithSameNameExistsException($"Team with the name:{teamToUpdate.Name} already exists!");

            teamToUpdate.ChangeDate = DateTime.Now;
            // foundTeam.UpdaterId = currentUser; //TODO

            await _teamRepository.CreateOrUpdate(teamToUpdate);
        }
        public async Task DeleteTeam(Guid teamId)
        {
            Team teamToDelete = await GetTeamWithId(teamId);

            await _teamRepository.Remove(teamToDelete);
        }
        public async Task UpdateTeamLeader(Guid teamId, User user)
        {
            Team foundTeam = await GetTeamWithId(teamId);

            foundTeam.ChangeDate = DateTime.Now;
            //foundTeam.UpdaterId = currentUser; // TODO

            foundTeam.TeamLeader = user;

            await _teamRepository.CreateOrUpdate(foundTeam);
        }
        public async Task AddUserToTeam(Guid teamId, User user)
        {
            Team foundTeam = await GetTeamWithId(teamId);

            foundTeam.Members.Add(user);

            await _teamRepository.SaveChanges();
        }
        public async Task<List<User>> GetAllTeamMembers(Guid teamId)
        {
            Team foundTeam = await GetTeamWithId(teamId);

            return foundTeam.Members;
        }
        public async Task RemoveUserFromTeam(Guid teamId, User user)
        {
            Team foundTeam = await GetTeamWithId(teamId);

            if (foundTeam.Members.Any(tempUser => tempUser.Id == user.Id) == false) // User we want to remove, isn't part of the team
                throw new KeyNotFoundException($" User with id:{user.Id} isnt part of this team!");

            foundTeam.Members.Remove(user);

            await _teamRepository.SaveChanges();
        }
    }
}
