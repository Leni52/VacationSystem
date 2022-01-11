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
        [Fact]
        public async Task Create_ValidTeam_Passes()
        {
            //arrange
            var teamRepositoryMock = new Mock<IRepository<Team>>();
            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Expression<Func<Team, bool>>>()))
                .ReturnsAsync((Team)null);

            var sut = new TeamService(teamRepositoryMock.Object);

            Team teamToAdd = new Team();

            //act
            var result = await Record.ExceptionAsync(() => sut.Create(teamToAdd));

            Assert.Null(result);
            //asert
        }
        [Fact]
        public async Task Create_TeamNameAlreadyExists_ThrowsException()
        {
            //arrange
            var teamRepositoryMock = new Mock<IRepository<Team>>();
            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Expression<Func<Team, bool>>>()))
                .ReturnsAsync(new Team());

            var sut = new TeamService(teamRepositoryMock.Object);

            Team teamToAdd = new Team();

            //act
            //asert
            await Assert.ThrowsAsync<TeamWithSameNameExistsException>(() => sut.Create(teamToAdd));
        }

        [Fact]
        public async Task GetTeamWithId_ValidId_ReturnsTeam()
        {
            //arrange
            var teamRepositoryMock = new Mock<IRepository<Team>>();
            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new Team());

            var sut = new TeamService(teamRepositoryMock.Object);

            //act
            //asert
            Assert.IsType<Team>(await sut.GetTeamWithId(Guid.NewGuid()));
        }

        [Fact]
        public async Task GetTeamWithId_InvalidId_ThrowsException()
        {
            //arrange
            var teamRepositoryMock = new Mock<IRepository<Team>>();
            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Expression<Func<Team, bool>>>()))
                .ReturnsAsync(new Team());

            var sut = new TeamService(teamRepositoryMock.Object);

            Team teamToAdd = new Team();

            //act
            //asert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.GetTeamWithId(Guid.NewGuid()));
        }

        [Fact]
        public async Task GetAllTeams_Valid_ReturnsListOfTeams()
        {
            //arrange
            var teamRepositoryMock = new Mock<IRepository<Team>>();
            teamRepositoryMock.Setup(teamRep => teamRep.All())
                .ReturnsAsync(new List<Team>());

            var sut = new TeamService(teamRepositoryMock.Object);

            //act
            //asert
            Assert.IsType<List<Team>>(await sut.GetAllTeams());
        }

        [Fact]
        public async Task UpdateTeam_ValidInput_Passes()
        {
            //arrange
            var teamRepositoryMock = new Mock<IRepository<Team>>();

            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Expression<Func<Team, bool>>>()))
                .ReturnsAsync((Team)null);
            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new Team());

            var sut = new TeamService(teamRepositoryMock.Object);

            //act
            var result = await Record.ExceptionAsync(() => sut.UpdateTeam(new Team(), Guid.NewGuid()));
            //asert
            Assert.Null(result); // assert no exception was thrown
        }

        [Fact]
        public async Task UpdateTeam_InvalidTeamId_ThrowsException()
        {
            //arrange
            var teamRepositoryMock = new Mock<IRepository<Team>>();

            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Expression<Func<Team, bool>>>()))
                .ReturnsAsync((Team)null);
            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync((Team)null);

            var sut = new TeamService(teamRepositoryMock.Object);

            //act
            //asert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.UpdateTeam(new Team(), Guid.NewGuid()));
        }
        [Fact]
        public async Task UpdateTeam_TeamWithSameNameExists_ThrowsException()
        {
            //arrange
            var teamRepositoryMock = new Mock<IRepository<Team>>();

            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Expression<Func<Team, bool>>>()))
                .ReturnsAsync(new Team());
            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new Team());

            var sut = new TeamService(teamRepositoryMock.Object);

            //act
            //asert
            await Assert.ThrowsAsync<TeamWithSameNameExistsException>(() => sut.UpdateTeam(new Team(), Guid.NewGuid()));
        }

        [Fact]
        public async Task DeleteTeam_ValidInput_Passes()
        {
            //arrange
            var teamRepositoryMock = new Mock<IRepository<Team>>();

            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new Team());

            var sut = new TeamService(teamRepositoryMock.Object);

            //act
            var result = await Record.ExceptionAsync(() => sut.DeleteTeam(Guid.NewGuid()));
            //asert
            Assert.Null(result); // assert no exception was thrown
        }

        [Fact]
        public async Task DeleteTeam_InvalidTeamId_ThrowsException()
        {
            //arrange
            var teamRepositoryMock = new Mock<IRepository<Team>>();

            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync((Team)null);

            var sut = new TeamService(teamRepositoryMock.Object);

            //act
            //asert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.DeleteTeam(Guid.NewGuid()));
        }

        [Fact]
        public async Task UpdateTeamLeader_ValidInput_Passes()
        {
            //arrange
            var teamRepositoryMock = new Mock<IRepository<Team>>();

            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new Team());

            var sut = new TeamService(teamRepositoryMock.Object);

            //act
            var result = await Record.ExceptionAsync(() => sut.UpdateTeamLeader(Guid.NewGuid(), new User()));
            //asert
            Assert.Null(result); // assert no exception was thrown
        }

        [Fact]
        public async Task UpdateTeamLeader_InvalidTeamId_ThrowsException()
        {
            //arrange
            var teamRepositoryMock = new Mock<IRepository<Team>>();

            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync((Team)null);

            var sut = new TeamService(teamRepositoryMock.Object);

            //act
            //asert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.UpdateTeamLeader(Guid.NewGuid(), new User()));
        }

        [Fact]
        public async Task AddUserToTeam_ValidInput_Passes()
        {
            //arrange
            var teamRepositoryMock = new Mock<IRepository<Team>>();

            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new Team());

            var sut = new TeamService(teamRepositoryMock.Object);

            //act
            var result = await Record.ExceptionAsync(() => sut.AddUserToTeam(Guid.NewGuid(), new User()));
            //asert
            Assert.Null(result); // assert no exception was thrown
        }

        [Fact]
        public async Task AddUserToTeam_InvalidTeamId_ThrowsException()
        {
            //arrange
            var teamRepositoryMock = new Mock<IRepository<Team>>();

            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync((Team)null);

            var sut = new TeamService(teamRepositoryMock.Object);

            //act
            //asert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.AddUserToTeam(Guid.NewGuid(), new User()));
        }

        [Fact]
        public async Task GetAllTeamMembers_ValidInput_Passes()
        {
            //arrange
            var teamRepositoryMock = new Mock<IRepository<Team>>();

            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new Team());

            var sut = new TeamService(teamRepositoryMock.Object);

            //act
            var result = await Record.ExceptionAsync(() => sut.GetAllTeamMembers(Guid.NewGuid()));
            //asert
            Assert.Null(result); // assert no exception was thrown
        }

        [Fact]
        public async Task GetAllTeamMembers_InvalidTeamId_ThrowsException()
        {
            //arrange
            var teamRepositoryMock = new Mock<IRepository<Team>>();

            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync((Team)null);

            var sut = new TeamService(teamRepositoryMock.Object);

            //act
            //asert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.GetAllTeamMembers(Guid.NewGuid()));
        }

        [Fact]
        public async Task RemoveUserFromTeam_ValidInput_Passes()
        {
            //arrange
            var teamRepositoryMock = new Mock<IRepository<Team>>();

            Team team = new Team() { Id = Guid.NewGuid()};
            User teamMember = new User() { Id = Guid.NewGuid().ToString() };
            team.Members.Add(teamMember);

            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync(team);

            var sut = new TeamService(teamRepositoryMock.Object);

            //act
            var result = await Record.ExceptionAsync(() => sut.RemoveUserFromTeam(team.Id, teamMember));
            //asert
            Assert.Null(result); // assert no exception was thrown
        }

        [Fact]
        public async Task RemoveUserFromTeam_InvalidTeamId_ThrowsException()
        {
            //arrange
            var teamRepositoryMock = new Mock<IRepository<Team>>();

            Team team = new Team() { Id = Guid.NewGuid() };
            User teamMember = new User() { Id = Guid.NewGuid().ToString() };
            team.Members.Add(teamMember);

            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync((Team)null);

            var sut = new TeamService(teamRepositoryMock.Object);

            //act
            await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.RemoveUserFromTeam(team.Id, teamMember));
        }

        [Fact]
        public async Task RemoveUserFromTeam_UserIsntPartOfTeam_ThrowsException()
        {
            //arrange
            var teamRepositoryMock = new Mock<IRepository<Team>>();

            Team team = new Team() { Id = Guid.NewGuid() };
            User teamMember = new User() { Id = Guid.NewGuid().ToString() };

            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync(team);

            var sut = new TeamService(teamRepositoryMock.Object);

            //act
            await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.RemoveUserFromTeam(team.Id, teamMember));
        }
    }
}
