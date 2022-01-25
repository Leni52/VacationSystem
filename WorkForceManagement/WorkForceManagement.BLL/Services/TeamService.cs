using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task Create(Team team, User currentUser)
        {
            if (await _teamRepository.Get(team => team.Name == team.Name) != null)
                throw new TeamWithSameNameExistsException($"Team with the name:{team.Name} already exists!");

            ValidateUserConfirmedEmail(team.TeamLeader);

            team.CreationDate = DateTime.Now;
            team.ChangeDate = DateTime.Now;
            team.CreatorId = currentUser.Id;
            team.UpdaterId = currentUser.Id;

            await _teamRepository.CreateOrUpdate(team);
        }

        private void ValidateUserConfirmedEmail(User user)
        {
            if (user.EmailConfirmed == false)
                throw new UserEmailNotConfirmedException($"Request cant be made, User with Id: {user.Id}, doesn't have email address confirmed");
        }

        public async Task<Team> GetTeamWithId(Guid id)
        {
            var team = await _teamRepository.Get(id);
            if (team == null)
                throw new KeyNotFoundException($"Team with id:{id} does not exist!");

            return team;
        }

        public async Task<List<Team>> GetAllTeams()
        {
            return await _teamRepository.All();
        }

        public async Task UpdateTeam(Team team, Guid teamId, User currentUser)
        {
            var teamWithSameName = await _teamRepository.Get(
                team =>
                team.Name == team.Name &&
                team.Id != teamId); // find different team with same name

            if (teamWithSameName != null)
                throw new TeamWithSameNameExistsException($"Team with the name:{team.Name} already exists!");

            ValidateUserConfirmedEmail(team.TeamLeader);

            team.ChangeDate = DateTime.Now;
            team.UpdaterId = currentUser.Id;

            await _teamRepository.CreateOrUpdate(team);
        }

        public async Task DeleteTeam(Guid id)
        {
            var teamToDelete = await GetTeamWithId(id);

            await _teamRepository.Remove(teamToDelete);
        }

        public async Task UpdateTeamLeader(Guid id, User user, User currentUser)
        {
            var team = await GetTeamWithId(id);

            ValidateUserConfirmedEmail(user);

            team.ChangeDate = DateTime.Now;
            team.UpdaterId = currentUser.Id;

            team.TeamLeader = user;

            await _teamRepository.CreateOrUpdate(team);
        }

        public async Task AddUserToTeam(Guid id, User user, User currentUser)
        {
            var team = await GetTeamWithId(id);
            if ((team.Members.Any(tempUser => tempUser.Id == user.Id)) ||
                (team.TeamLeader.Id == user.Id))
                // User we want to add, is part of the team
                throw new ItemAlreadyExistsException($" User with id:{user.Id} is part of this team!");
            ValidateUserConfirmedEmail(user);

            team.Members.Add(user);
            team.UpdaterId = currentUser.Id;
            team.ChangeDate = DateTime.Now.Date;

            await _teamRepository.SaveChanges();
        }

        public async Task<List<User>> GetAllTeamMembers(Guid id)
        {
            var team = await GetTeamWithId(id);
            var members = team.Members;
            members.Add(team.TeamLeader);
            return members;
        }

        public async Task RemoveUserFromTeam(Guid id, User user, User currentUser)
        {
            var team = await GetTeamWithId(id);

            if (!team.Members.Any(tempUser => tempUser.Id == user.Id))
                // User we want to remove, isn't part of the team
                throw new KeyNotFoundException($" User with id:{user.Id} isnt part of this team!");
            team.Members.Remove(user);
            team.UpdaterId = currentUser.Id;
            team.ChangeDate = DateTime.Now.Date;

            await _teamRepository.SaveChanges();
        }
    }
}