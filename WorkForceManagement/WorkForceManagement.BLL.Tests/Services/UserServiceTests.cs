using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkForceManagement.BLL.Exceptions;
using WorkForceManagement.BLL.Services;
using WorkForceManagement.DAL.Entities;
using Xunit;

namespace WorkForceManagement.BLL.Tests.Services
{
    public class UserServiceTests
    {
        PasswordHasher<User> hasher = new PasswordHasher<User>();

        [Fact]
        public async Task CreateUser_UsernameTaken_ThrowsException()
        {
            //arrange
            var authUserManagerMock = new Mock<IAuthUserManager>();
            var teamServiceMock = new Mock<ITeamService>();

            authUserManagerMock.Setup(userRep => userRep.FindDifferentUserWithSameUsername(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(new User());

            var sut = new UserService(teamServiceMock.Object, authUserManagerMock.Object);

            User currentUser = new User()
            {
                UserName = "admin",
            };
            currentUser.PasswordHash = hasher.HashPassword(currentUser, "adminpass");

            User userToAdd = new User()
            {
                UserName = "admin",
            };
            userToAdd.PasswordHash = hasher.HashPassword(userToAdd, "adminpass");

            //act
            //assert
            await Assert.ThrowsAsync<UsernameTakenException>(() => sut.Add(userToAdd, "adminpass", true));
        }
        
        [Fact]
        public async Task CreateUser_ValidUser_Passes()
        {
            //arrange
            var authUserManagerMock = new Mock<IAuthUserManager>();
            var teamServiceMock = new Mock<ITeamService>();

            var sut = new UserService(teamServiceMock.Object, authUserManagerMock.Object);

            User currentUser = new User()
            {
                UserName = "admin",
                Email = "test1@gmail.com"
            };
            currentUser.PasswordHash = hasher.HashPassword(currentUser, "adminpass");


            User userToAdd = new User()
            {
                UserName = "jack",
                Email = "test2@gmail.com"
            };
            userToAdd.PasswordHash = hasher.HashPassword(userToAdd, "jack123");

            //act
            var exception = await Record.ExceptionAsync(() => sut.Add(userToAdd, "jack123", true));
            //assert
            Assert.Null(exception);
        }

        [Fact]
        public async Task Delete_ValidUser_Passes()
        {
            //arrange
            var authUserManagerMock = new Mock<IAuthUserManager>();
            var teamServiceMock = new Mock<ITeamService>();

            authUserManagerMock.Setup(userRep => userRep.FindById(It.IsAny<Guid>()))
                .ReturnsAsync(new User());

            var sut = new UserService(teamServiceMock.Object, authUserManagerMock.Object);

            Guid userId = Guid.NewGuid();
            //act
            var exception = await Record.ExceptionAsync(() => sut.Delete(userId));
            Assert.Null(exception);
        }

        [Fact]
        public async Task Deletec_NotFoundUser_ThrowsException()
        {
            //arrange
            var authUserManagerMock = new Mock<IAuthUserManager>();
            var teamServiceMock = new Mock<ITeamService>();

            authUserManagerMock.Setup(userRep => userRep.FindById(It.IsAny<Guid>()))
                .ReturnsAsync((User)null);

            var sut = new UserService(teamServiceMock.Object, authUserManagerMock.Object);

            Guid userId = Guid.NewGuid();
            //act
            await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Delete(userId));
        }

        [Fact]
        public async Task Edit_ValidEntry_ReturnsTrue()
        {
            //arrange
            var authUserManagerMock = new Mock<IAuthUserManager>();
            var teamServiceMock = new Mock<ITeamService>();

            authUserManagerMock.Setup(userRep => userRep.FindDifferentUserWithSameUsername(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync((User)null);
            authUserManagerMock.Setup(userRep => userRep.FindById(It.IsAny<Guid>()))
                .ReturnsAsync(new User());

            var sut = new UserService(teamServiceMock.Object, authUserManagerMock.Object);

            User editedUser = new User()
            {
                UserName = "jack",
            };
            editedUser.PasswordHash = hasher.HashPassword(editedUser, "jack123");

            Guid userId = Guid.NewGuid();
            //act
            var exception = await Record.ExceptionAsync(() => sut.Update(editedUser, "jack", true));
            Assert.Null(exception);
        }

        [Fact]
        public async Task GetAllUsers_ValidEntry_Passes()
        {
            //arrange
            var authUserManagerMock = new Mock<IAuthUserManager>();
            var teamServiceMock = new Mock<ITeamService>();

            authUserManagerMock.Setup(userRep => userRep.GetAll())
                .ReturnsAsync(new List<User>());

            var sut = new UserService(teamServiceMock.Object, authUserManagerMock.Object);

            //act
            var exception = await Record.ExceptionAsync(() => sut.GetAllUsers());
            Assert.Null(exception);
        }

        [Fact]
        public async Task GetUserWithId_InvalidIdNotFoundUser_ThrowsException()
        {
            //arrange
            var authUserManagerMock = new Mock<IAuthUserManager>();
            var teamServiceMock = new Mock<ITeamService>();

            authUserManagerMock.Setup(userRep => userRep.FindById(It.IsAny<Guid>()))
                .ReturnsAsync((User)null);

            var sut = new UserService(teamServiceMock.Object, authUserManagerMock.Object);

            Guid userId = Guid.NewGuid();

            await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.GetUserById(userId));
        }
        
    }
}
