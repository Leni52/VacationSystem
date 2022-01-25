using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WorkForceManagement.BLL.Exceptions;
using WorkForceManagement.BLL.Services;
using WorkForceManagement.DAL.Entities;
using WorkForceManagement.DAL.Repositories;
using Xunit;

namespace WorkForceManagement.BLL.Tests.Services
{
    public class TeamServiceTest
    {
        private readonly Mock<IRepository<Team>> teamRepositoryMock = new Mock<IRepository<Team>>();
        private readonly TeamService sut;

        public TeamServiceTest()
        {
            sut = new TeamService(teamRepositoryMock.Object);
        }


        [Fact]
        public async Task Create_ValidTeam_Passes()
        {
            //arrange
            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Expression<Func<Team, bool>>>()))
                .ReturnsAsync((Team)null);

            var teamLeader = new User() { EmailConfirmed = true };
            Team teamToAdd = new Team() { TeamLeader = teamLeader};

            //act
            var result = await Record.ExceptionAsync(() => sut.Create(teamToAdd, teamLeader));

            Assert.Null(result);
            //asert
        }
        [Fact]
        public async Task Create_TeamNameAlreadyExists_ThrowsException()
        {
            //arrange
            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Expression<Func<Team, bool>>>()))
                .ReturnsAsync(new Team());

            Team teamToAdd = new Team();

            //act
            //asert
            await Assert.ThrowsAsync<TeamWithSameNameExistsException>(() => sut.Create(teamToAdd, new User()));
        }

        [Fact]
        public async Task GetTeamWithId_ValidId_ReturnsTeam()
        {
            //arrange
            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new Team());

            //act
            //asert
            Assert.IsType<Team>(await sut.GetTeamWithId(Guid.NewGuid()));
        }

        [Fact]
        public async Task GetTeamWithId_InvalidId_ThrowsException()
        {
            //arrange
            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Expression<Func<Team, bool>>>()))
                .ReturnsAsync(new Team());

            Team teamToAdd = new Team();

            //act
            //asert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.GetTeamWithId(Guid.NewGuid()));
        }

        [Fact]
        public async Task GetAllTeams_Valid_ReturnsListOfTeams()
        {
            //arrange
            teamRepositoryMock.Setup(teamRep => teamRep.All())
                .ReturnsAsync(new List<Team>());

            //act
            //asert
            Assert.IsType<List<Team>>(await sut.GetAllTeams());
        }

        [Fact]
        public async Task UpdateTeam_ValidInput_Passes()
        {
            //arrange
            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Expression<Func<Team, bool>>>()))
                .ReturnsAsync((Team)null);
            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new Team());

            var teamLeader = new User() { EmailConfirmed = true };
            var teamToUpdate = new Team() { TeamLeader = teamLeader };

            //act
            var result = await Record.ExceptionAsync(() => sut.UpdateTeam(teamToUpdate, Guid.NewGuid(), new User()));
            //asert
            Assert.Null(result); // assert no exception was thrown
        }

        [Fact]
        public async Task UpdateTeam_TeamWithSameNameExists_ThrowsException()
        {
            //arrange
            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Expression<Func<Team, bool>>>()))
                .ReturnsAsync(new Team());
            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new Team());

            //act
            //asert
            await Assert.ThrowsAsync<TeamWithSameNameExistsException>(() => sut.UpdateTeam(new Team(), Guid.NewGuid(), new User()));
        }
        [Fact]
        public async Task UpdateTeam_TeamLeaderDidntConfirmEmail_ThrowsException()
        {
            //arrange
            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Expression<Func<Team, bool>>>()))
                .ReturnsAsync((Team)null);
            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new Team());

            var teamLeader = new User() { EmailConfirmed = false };
            var teamToUpdate = new Team() { TeamLeader = teamLeader };

            //act
            //asert
            await Assert.ThrowsAsync<UserEmailNotConfirmedException>(() => sut.UpdateTeam(teamToUpdate, Guid.NewGuid(), new User()));
        }

        [Fact]
        public async Task DeleteTeam_ValidInput_Passes()
        {
            //arrange
            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new Team());
            //act
            var result = await Record.ExceptionAsync(() => sut.DeleteTeam(Guid.NewGuid()));
            //asert
            Assert.Null(result); // assert no exception was thrown
        }

        [Fact]
        public async Task DeleteTeam_InvalidTeamId_ThrowsException()
        {
            //arrange
            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync((Team)null);
            //act
            //asert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.DeleteTeam(Guid.NewGuid()));
        }

        [Fact]
        public async Task UpdateTeamLeader_ValidInput_Passes()
        {
            //arrange
            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new Team());

            var newTeamLeader = new User() { EmailConfirmed = true };
            //act
            var result = await Record.ExceptionAsync(() => sut.UpdateTeamLeader(Guid.NewGuid(), newTeamLeader, new User()));
            //asert
            Assert.Null(result); // assert no exception was thrown
        }

        [Fact]
        public async Task UpdateTeamLeader_InvalidTeamId_ThrowsException()
        {
            //arrange
            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync((Team)null);
            //act
            //asert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.UpdateTeamLeader(Guid.NewGuid(), new User(), new User()));
        }
        [Fact]
        public async Task UpdateTeamLeader_TeamLeaderDidntConfirmEmail_ThrowsException()
        {
            //arrange
            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new Team());

            var newTeamLeader = new User() { EmailConfirmed = false };
            //act
            //asert
            await Assert.ThrowsAsync<UserEmailNotConfirmedException>(() => sut.UpdateTeamLeader(Guid.NewGuid(), newTeamLeader, new User()));
        }

        [Fact]
        public async Task AddUserToTeam_ValidInput_Passes()
        {
            //arrange
            var team = new Team() { TeamLeader = new User(), Members = new List<User>() };
            var userToAdd = new User() { EmailConfirmed = true };

            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync(team);
            //act
            var result = await Record.ExceptionAsync(() => sut.AddUserToTeam(Guid.NewGuid(), userToAdd, new User()));
            //asert
            Assert.Null(result); // assert no exception was thrown
        }

        [Fact]
        public async Task AddUserToTeam_InvalidTeamId_ThrowsException()
        {
            //arrange
            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync((Team)null);

            //act
            //asert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.AddUserToTeam(Guid.NewGuid(), new User(), new User()));
        }

        [Fact]
        public async Task AddUserToTeam_UserAlreadyInTeam_ThrowException()
        {
            //arrange
            var userToAdd = new User() { EmailConfirmed = true };
            var team = new Team() { TeamLeader = new User(), Members = new List<User>{ userToAdd } };
            

            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync(team);
            //act
            //asert
            await Assert.ThrowsAsync<ItemAlreadyExistsException>(() => sut.AddUserToTeam(Guid.NewGuid(), userToAdd, new User()));
        }

        [Fact]
        public async Task AddUserToTeam_UserDidntConfirmEmail_ThrowException()
        {
            //arrange
            var team = new Team() { TeamLeader = new User(), Members = new List<User>() };
            var userToAdd = new User() { EmailConfirmed = false };


            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync(team);
            //act
            //asert
            await Assert.ThrowsAsync<UserEmailNotConfirmedException>(() => sut.AddUserToTeam(Guid.NewGuid(), userToAdd, new User()));
        }

        [Fact]
        public async Task GetAllTeamMembers_ValidInput_Passes()
        {
            //arrange
            var team = new Team() { Members = new List<User>(), TeamLeader = new User() };

            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync(team);

            //act
            var result = await Record.ExceptionAsync(() => sut.GetAllTeamMembers(Guid.NewGuid()));
            //asert
            Assert.Null(result); // assert no exception was thrown
        }

        [Fact]
        public async Task GetAllTeamMembers_InvalidTeamId_ThrowsException()
        {
            //arrange
            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync((Team)null);
            //act
            //asert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.GetAllTeamMembers(Guid.NewGuid()));
        }

        [Fact]
        public async Task RemoveUserFromTeam_ValidInput_Passes()
        {
            //arrange
            Team team = new Team() { Id = Guid.NewGuid(), Members = new List<User>()};
            User teamMember = new User() { Id = Guid.NewGuid().ToString() };
            team.Members.Add(teamMember);

            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync(team);
            //act
            var result = await Record.ExceptionAsync(() => sut.RemoveUserFromTeam(team.Id, teamMember, new User()));
            //asert
            Assert.Null(result); // assert no exception was thrown
        }

        [Fact]
        public async Task RemoveUserFromTeam_InvalidTeamId_ThrowsException()
        {
            //arrange
            Team team = new Team() { Id = Guid.NewGuid(), Members = new List<User>()};
            User teamMember = new User() { Id = Guid.NewGuid().ToString() };
            team.Members.Add(teamMember);

            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync((Team)null);
            //act
            await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.RemoveUserFromTeam(team.Id, teamMember, new User()));
        }

        [Fact]
        public async Task RemoveUserFromTeam_UserIsntPartOfTeam_ThrowsException()
        {
            //arrange
            Team team = new Team() { Id = Guid.NewGuid(), Members = new List<User>() };
            User teamMember = new User() { Id = Guid.NewGuid().ToString() };

            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync(team);
            //act
            await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.RemoveUserFromTeam(team.Id, teamMember, new User()));
        }

        [Fact]
        public async Task Create_TeamLeaderDidntConfirmEmail_ThrowsException()
        {
            //arrange
            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Expression<Func<Team, bool>>>()))
                .ReturnsAsync((Team)null);

            User teamLeader = new User()
            {
                EmailConfirmed = false
            };
            Team teamToAdd = new Team()
            {
                TeamLeader = teamLeader
            };

            //act
            //asert
            await Assert.ThrowsAsync<UserEmailNotConfirmedException>(() => sut.Create(teamToAdd, new User()));
        }

    }
}
