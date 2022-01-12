using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkForceManagement.BLL.Services;
using WorkForceManagement.DAL.Entities;
using WorkForceManagement.BLL.Exceptions;
using Xunit;

namespace WorkForceManagement.BLL.Tests.Services
{
    public class UserServiceTests
    {
        PasswordHasher<User> hasher = new PasswordHasher<User>();



        [Fact]
        public async Task CreateUser_ValidUser_Passes()
        {
            //arrange
            var authUserManagerMock = new Mock<IAuthUserManager>();

            var sut = new UserService(authUserManagerMock.Object);

            User currentUser = new User()
            {
                UserName = "admin",
            };
            currentUser.PasswordHash = hasher.HashPassword(currentUser, "adminpass");


            User userToAdd = new User()
            {
                UserName = "jakup",
            };
            userToAdd.PasswordHash = hasher.HashPassword(userToAdd, "emini");

            //act
            var exception = await Record.ExceptionAsync(() => sut.Add(userToAdd, "emini", true));
            //assert
            Assert.Null(exception); // assert ni exception is thrown
        }

        [Fact]
        public async Task CreateUser_UsernameTaken_ThrowsException()
        {
            //arrange
            var authUserManagerMock = new Mock<IAuthUserManager>();

            authUserManagerMock.Setup(userRep => userRep.FindDifferentUserWithSameUsername(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(new User());

            var sut = new UserService(authUserManagerMock.Object);

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
        public async Task DeleteAsync_ValidUser_Passes()
        {
            //arrange
            var authUserManagerMock = new Mock<IAuthUserManager>();

            authUserManagerMock.Setup(userRep => userRep.FindById(It.IsAny<Guid>()))
                .ReturnsAsync(new User());

            var sut = new UserService(authUserManagerMock.Object);

            Guid userId = Guid.NewGuid();
            //act
            var exception = await Record.ExceptionAsync(() => sut.Delete(userId));
            Assert.Null(exception); // sut throws no exception
        }

        [Fact]
        public async Task DeleteAsync_NotFoundUser_ThrowsException()
        {
            //arrange
            var authUserManagerMock = new Mock<IAuthUserManager>();
            authUserManagerMock.Setup(userRep => userRep.FindById(It.IsAny<Guid>()))
                .ReturnsAsync((User)null);

            var sut = new UserService(authUserManagerMock.Object);

            Guid userId = Guid.NewGuid();
            //act
            await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Delete(userId));
        }

        [Fact]
        public async Task EditAsync_ValidEntry_ReturnsTrue()
        {
            //arrange
            var authUserManagerMock = new Mock<IAuthUserManager>();

            authUserManagerMock.Setup(userRep => userRep.FindDifferentUserWithSameUsername(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync((User)null);
            authUserManagerMock.Setup(userRep => userRep.FindById(It.IsAny<Guid>()))
                .ReturnsAsync(new User());

            var sut = new UserService(authUserManagerMock.Object);

            User editedUser = new User()
            {
                UserName = "altin",
            };
            editedUser.PasswordHash = hasher.HashPassword(editedUser, "jakup");

            Guid userId = Guid.NewGuid();
            //act
            var exception = await Record.ExceptionAsync(() => sut.Edit(userId, editedUser, "altin", true));
            Assert.Null(exception);
        }

        [Fact]
        public async Task GetAllUsersAsync_ValidEntry_Passes()
        {
            //arrange
            var authUserManagerMock = new Mock<IAuthUserManager>();
            authUserManagerMock.Setup(userRep => userRep.GetAll())
                .ReturnsAsync(new List<User>());

            var sut = new UserService(authUserManagerMock.Object);

            //act
            var exception = await Record.ExceptionAsync(() => sut.GetAllUsers());
            Assert.Null(exception);
        }
    }
}
