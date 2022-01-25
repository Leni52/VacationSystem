using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkForceManagement.BLL.Exceptions;
using WorkForceManagement.BLL.Services;
using WorkForceManagement.BLL.Services.Interfaces;
using WorkForceManagement.DAL.Entities;
using Xunit;

namespace WorkForceManagement.BLL.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IAuthUserManager> authUserManagerMock = new Mock<IAuthUserManager>();
        private readonly Mock<IMailService> mailServiceMock = new Mock<IMailService>();
        private readonly Mock<ITeamService> teamServiceMock = new Mock<ITeamService>();
        private readonly UserService sut;

        PasswordHasher<User> hasher = new PasswordHasher<User>();

        public UserServiceTests()
        {
            sut = new UserService(teamServiceMock.Object, authUserManagerMock.Object, mailServiceMock.Object);
        }

        [Fact]
        public async Task CreateUser_UsernameTaken_ThrowsException()
        {
            //arrange

            authUserManagerMock.Setup(userRep => userRep.FindDifferentUserWithSameUsername(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(new User());

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

            authUserManagerMock.Setup(auth => auth.GenerateEmailConfirmationTokenAsync(It.IsAny<User>()))
                .ReturnsAsync("testToken");
            //act
            var exception = await Record.ExceptionAsync(() => sut.Add(userToAdd, "jack123", true));
            //assert
            Assert.Null(exception);
        }

        [Fact]
        public async Task Delete_ValidUser_Passes()
        {
            //arrange
            authUserManagerMock.Setup(userRep => userRep.FindById(It.IsAny<Guid>()))
                .ReturnsAsync(new User());

            Guid userId = Guid.NewGuid();
            //act
            var exception = await Record.ExceptionAsync(() => sut.Delete(userId));
            Assert.Null(exception);
        }

        [Fact]
        public async Task Deletec_NotFoundUser_ThrowsException()
        {
            //arrange
            authUserManagerMock.Setup(userRep => userRep.FindById(It.IsAny<Guid>()))
                .ReturnsAsync((User)null);

            Guid userId = Guid.NewGuid();
            //act
            await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Delete(userId));
        }

        [Fact]
        public async Task Edit_ValidEntry_ReturnsTrue()
        {
            //arrange
            authUserManagerMock.Setup(userRep => userRep.FindDifferentUserWithSameUsername(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync((User)null);
            authUserManagerMock.Setup(userRep => userRep.FindById(It.IsAny<Guid>()))
                .ReturnsAsync(new User());

            User editedUser = new User()
            {
                UserName = "jack",
                Email = "test1@gmail.com"
            };
            editedUser.PasswordHash = hasher.HashPassword(editedUser, "jack123");

            Guid userId = Guid.NewGuid();
            //act
            var exception = await Record.ExceptionAsync(() => sut.Update(editedUser,"test1@gmail.com", "jack", true));
            Assert.Null(exception);
        }

        [Fact]
        public async Task GetAllUsers_ValidEntry_Passes()
        {
            //arrange
            authUserManagerMock.Setup(userRep => userRep.GetAll())
                .ReturnsAsync(new List<User>());

            //act
            var exception = await Record.ExceptionAsync(() => sut.GetAllUsers());
            Assert.Null(exception);
        }

        [Fact]
        public async Task GetUserWithId_InvalidIdNotFoundUser_ThrowsException()
        {
            //arrange
            authUserManagerMock.Setup(userRep => userRep.FindById(It.IsAny<Guid>()))
                .ReturnsAsync((User)null);

            Guid userId = Guid.NewGuid();

            await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.GetUserById(userId));
        }

        [Fact]
        public async Task IsEmailValid_InvalidEmail_ThrowsException()
        {
            //arrange
            User currentUser = new User()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "admin",
                Email = "badEmail"
            };
            currentUser.PasswordHash = hasher.HashPassword(currentUser, "adminpass");

            authUserManagerMock.Setup(userRep => userRep.FindDifferentUserWithSameUsername(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync((User)null);


            await Assert.ThrowsAsync<InvalidEmailException>(() => sut.Add(currentUser, "adminpass", true));
        }

        [Fact]
        public async Task IsEmailValid_EmailAlreadyInUse_ThrowsException()
        {
            //arrange
            User userToAdd = new User()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "admin",
                Email = "test@gmail.com"
            };
            userToAdd.PasswordHash = hasher.HashPassword(userToAdd, "adminpass");

            authUserManagerMock.Setup(userRep => userRep.FindDifferentUserWithSameUsername(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync((User)null);
            authUserManagerMock.Setup(userRep => userRep.FindByEmail(It.IsAny<string>()))
                .ReturnsAsync(new User());

            await Assert.ThrowsAsync<EmailAddressAlreadyInUseException>(() => sut.Add(userToAdd, "adminpass", true));
        }

        [Fact]
        public async Task Update_ValidInput_Passes()
        {
            User userToUpdate = new User()
            {
                Id = Guid.NewGuid().ToString(),
                Email = "test@gmail.com",
                UserName = "testUser",
                PasswordHash = "testPass"
            };
            var exception = await Record.ExceptionAsync(() => sut.Update(userToUpdate, "test@gmail.com", "testPass", true));
            Assert.Null(exception);
        }

        [Fact]
        public async Task Update_UsernameTaken_ThrowsException()
        {
            //arrange
            User userToUpdate = new User()
            {
                Id = Guid.NewGuid().ToString(),
                Email = "test@gmail.com",
                UserName = "testUser",
                PasswordHash = "testPass"
            };
            authUserManagerMock.Setup(manager => manager.FindDifferentUserWithSameUsername(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(new User());
            //act
            //assert
            await Assert.ThrowsAsync<UsernameTakenException>(() => sut.Update(userToUpdate, "test@gmail.com", "testPass", true));
        }
        [Fact]
        public async Task Update_EmailAddresAlreadyInUse_ThrowsException()
        {
            //arrange
            User userToUpdate = new User()
            {
                Id = Guid.NewGuid().ToString(),
                Email = "test@gmail.com",
                UserName = "testUser",
            };
            User userWithSameEmail = new User()
            {
                Id = Guid.NewGuid().ToString(),
                Email = "test@gmail.com",
                UserName = "otherUser",
            };
            authUserManagerMock.Setup(manager => manager.FindByEmail(It.IsAny<string>()))
                .ReturnsAsync(userWithSameEmail);

            //act
            //assert
            await Assert.ThrowsAsync<EmailAddressAlreadyInUseException>(() => sut.Update(userToUpdate, "test@gmail.com", "testPass", true));
        }

        [Fact]
        public async Task IsUserAdmin_UserIsAdmin_ReturnsTrue()
        {
            User currentUser = new User();

            authUserManagerMock.Setup(manager => manager.IsUserInRole(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            Assert.True(await sut.IsUserAdmin(currentUser));
        }

        [Fact]
        public async Task IsUserAdmin_UserIsNotAdmin_ReturnsFalse()
        {
            User currentUser = new User();

            authUserManagerMock.Setup(manager => manager.IsUserInRole(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            Assert.False(await sut.IsUserAdmin(currentUser));
        }


    }
}
