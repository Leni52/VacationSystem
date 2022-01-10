using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkForceManagement.BLL.Services;
using WorkForceManagement.DAL.Entities;
using WorkForceManagement.DAL.Repositories;
using Xunit;

namespace WorkForceManagement.BLL.Tests.Services
{
    public class TeamServiceTest
    {
        [Fact]
        public async Task CreateAsync_ValidTeam_Passes()
        {
            //arrange
            var teamRepositoryMock = new Mock<IRepository<Team>>();
            teamRepositoryMock.Setup(teamRep => teamRep.Get(It.IsAny<Func<Team, bool>>()))
                .Returns((Team)null);

            var sut = new TeamService(teamRepositoryMock.Object);

            Team teamToAdd = new Team();

            //act
            var result = await Record.ExceptionAsync(() => sut.CreateAsync(teamToAdd));

            Assert.Null(result);
            //asert
        }
    }
}
