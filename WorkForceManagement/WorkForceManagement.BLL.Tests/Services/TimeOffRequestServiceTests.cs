using Moq;
using System;
using System.Collections.Generic;
using WorkForceManagement.BLL.Exceptions;
using WorkForceManagement.BLL.Services;
using WorkForceManagement.DAL.Entities;
using WorkForceManagement.DAL.Repositories;
using Xunit;

namespace WorkForceManagement.BLL.Tests.Services
{
    public class TimeOffRequestServiceTests
    {
        private readonly Mock<IRepository<TimeOffRequest>> requestRepositoryStub = new Mock<IRepository<TimeOffRequest>>();
        private readonly Mock<IUserService> userServiceMock = new Mock<IUserService>();
        private readonly Mock<ITeamService> teamServiceMock = new Mock<ITeamService>();
        private readonly Mock<IMailService> mailService = new Mock<IMailService>();
        private readonly TimeOffRequestService sut;

        public TimeOffRequestServiceTests()
        {
            sut = new TimeOffRequestService(requestRepositoryStub.Object, userServiceMock.Object, teamServiceMock.Object, mailService.Object);
        }

        //create
        [Fact]
        public void Create_ValidRequest_Pass()
        {
            //arrange
            User currentUser = new User()
            {
                UserName = "admin",
            };

            TimeOffRequest timeOffRequest = new TimeOffRequest()
            {
                StartDate = new DateTime(2022, 1, 22),
                EndDate = new DateTime(2022, 1, 28),
                Description = "Testing and testing",
                Type = TimeOffRequestType.Paid
            };
            //act
            var result = sut.CreateTimeOffRequest(timeOffRequest, currentUser);

            Assert.NotNull(result);
            //asert
        }
        
        [Fact]
        public void Create_InvalidRequest_Fail()
        {
            //arrange
            User currentUser = null;

            //act
            var result = sut.CreateTimeOffRequest(new TimeOffRequest(), currentUser);
            Assert.False(result.IsCompletedSuccessfully);
            //asert
        }

        
        [Fact]
        public void CreateRequest_InvalidRequest_Fail()
        {
            //arrange
            User currentUser = new User() { UserName = "Admin" };
            TimeOffRequest t1 = new TimeOffRequest()
            {
                Description = "Testing and testing"
            };

            //act
            var result = sut.CreateTimeOffRequest(t1, currentUser);
            //asert
            Assert.False(result.IsCompletedSuccessfully);
        }
        //delete
        [Fact]
        public void Delete_ValidId_Pass()
        {
            //arrange
            TimeOffRequest timeOffRequest = new TimeOffRequest();
            //act
            var result = sut.DeleteTimeOffRequest(timeOffRequest.Id);

            Assert.NotNull(result);
            //asert
        }

        [Fact]
        public void Delete_InvalidId_Fail()
        {
            //arrange
            TimeOffRequest timeOffRequest = new TimeOffRequest();
            //act
            //assert           
            Assert.ThrowsAsync<ItemDoesNotExistException>(() => sut.DeleteTimeOffRequest(Guid.NewGuid()));
        }

        //get
        [Fact]
        public async void GetAllRequests_ValidRequest_Pass()
        {
            //arrange
            requestRepositoryStub.Setup(reqRep => reqRep.All())
              .ReturnsAsync(new List<TimeOffRequest>());
            User currentUser = new User()
            { UserName = "Admin" };

            //act
            var result = await sut.GetAllRequests();
            //asert
            Assert.IsType<List<TimeOffRequest>>(result);
        }

        [Fact]
        public async void GetRequest_ValidRequest_Pass()
        {
            //arrange
            User currentUser = new User()
            {
                UserName = "Admin"
            };
            requestRepositoryStub.Setup(reqRep => reqRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new TimeOffRequest());

            //act
            var result = await sut.GetTimeOffRequest(Guid.NewGuid());
            //asert
            Assert.IsType<TimeOffRequest>(result);
        }

        [Fact]
        public async void GetRequest_InvalidRequest_Fail()
        {
            //arrange
            //act           
            //asert         
            await Assert.ThrowsAsync<ItemDoesNotExistException>(() => sut.GetTimeOffRequest(Guid.NewGuid()));

        }
        //update
        [Fact]
        public async void UpdateRequest_ValidRequest_Pass()
        {
            //arrange
            var currentUser = new User() { UserName = "Admin", DaysOff=20, CreatedTimeOffRequests = new List<TimeOffRequest>() };
            userServiceMock.Setup(user => user.GetUserById(It.IsAny<Guid>()))
                .ReturnsAsync(currentUser);
            requestRepositoryStub.Setup(reqRep => reqRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new TimeOffRequest());
            //act
            var result = await Record.ExceptionAsync(() => sut.UpdateTimeOffRequest(new Guid(),
                new TimeOffRequest(), currentUser.Id));
            //assert
            Assert.Null(result);
        }
       
    }
}
