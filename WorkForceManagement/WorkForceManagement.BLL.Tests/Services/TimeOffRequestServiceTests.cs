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
        //create
        [Fact]
        public void Create_ValidRequest_Pass()
        {
            //arrange
            var requestRepositoryStub = new Mock<IRepository<TimeOffRequest>>();
            var userServiceMock = new Mock<IUserService>();
            var mailService = new Mock<IMailService>();
            User currentUser = new User()
            {
                UserName = "admin",
            };

            var sut = new TimeOffRequestService(requestRepositoryStub.Object,userServiceMock.Object, mailService.Object);

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
            var requestRepositoryStub = new Mock<IRepository<TimeOffRequest>>();
            var userServiceMock = new Mock<IUserService>();
            var mailService = new Mock<IMailService>();
            User currentUser = null;

            var sut = new TimeOffRequestService(requestRepositoryStub.Object, userServiceMock.Object, mailService.Object);
            //act
            var result = sut.CreateTimeOffRequest(new TimeOffRequest(), currentUser);
            Assert.False(result.IsCompletedSuccessfully);
            //asert
        }
        [Fact]
        public void CreateRequest_InvalidRequest_Fail()
        {
            //arrange
            var requestRepositoryStub = new Mock<IRepository<TimeOffRequest>>();
            var userServiceMock = new Mock<IUserService>();
            var mailService = new Mock<IMailService>();
            User currentUser = new User() { UserName = "Admin" };
            TimeOffRequest t1 = new TimeOffRequest()
            {
                Description = "Testing and testing"
            };

            var sut = new TimeOffRequestService(requestRepositoryStub.Object, userServiceMock.Object, mailService.Object);
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
            var requestRepositoryStub = new Mock<IRepository<TimeOffRequest>>();
            var userServiceMock = new Mock<IUserService>();
            var mailService = new Mock<IMailService>();

            var sut = new TimeOffRequestService(requestRepositoryStub.Object, userServiceMock.Object, mailService.Object);

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
            var requestRepositoryStub = new Mock<IRepository<TimeOffRequest>>();
            var userServiceMock = new Mock<IUserService>();
            var mailService = new Mock<IMailService>();

            var sut = new TimeOffRequestService(requestRepositoryStub.Object, userServiceMock.Object, mailService.Object);

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
            var requestRepositoryStub = new Mock<IRepository<TimeOffRequest>>();
            var userServiceMock = new Mock<IUserService>();
            var mailService = new Mock<IMailService>();
            requestRepositoryStub.Setup(reqRep => reqRep.All())
              .ReturnsAsync(new List<TimeOffRequest>());
            User currentUser = new User()
            { UserName = "Admin" };

            var sut = new TimeOffRequestService(requestRepositoryStub.Object, userServiceMock.Object, mailService.Object);

            //act
            var result = await sut.GetAllRequests();
            //asert
            Assert.IsType<List<TimeOffRequest>>(result);
        }

        [Fact]
        public async void GetRequest_ValidRequest_Pass()
        {
            //arrange
            var requestRepositoryStub = new Mock<IRepository<TimeOffRequest>>();
            var userServiceMock = new Mock<IUserService>();
            var mailService = new Mock<IMailService>();
            User currentUser = new User()
            {
                UserName = "Admin"
            };
            requestRepositoryStub.Setup(reqRep => reqRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new TimeOffRequest());

            var sut = new TimeOffRequestService(requestRepositoryStub.Object, userServiceMock.Object, mailService.Object);

            //act
            var result = await sut.GetTimeOffRequest(Guid.NewGuid());
            //asert
            Assert.IsType<TimeOffRequest>(result);
        }

        [Fact]
        public async void GetRequest_InvalidRequest_Fail()
        {
            //arrange
            var requestRepositoryStub = new Mock<IRepository<TimeOffRequest>>();
            var userServiceMock = new Mock<IUserService>();
            var mailService = new Mock<IMailService>();

            var sut = new TimeOffRequestService(requestRepositoryStub.Object, userServiceMock.Object, mailService.Object);
            //act           
            //asert         
            await Assert.ThrowsAsync<ItemDoesNotExistException>(() => sut.GetTimeOffRequest(Guid.NewGuid()));

        }
        //update
        [Fact]
        public async void UpdateRequest_ValidRequest_Pass()
        {
            //arrange
            var requestRepositoryStub = new Mock<IRepository<TimeOffRequest>>();
            var userServiceMock = new Mock<IUserService>();
            var mailService = new Mock<IMailService>();

            var sut = new TimeOffRequestService(requestRepositoryStub.Object, userServiceMock.Object, mailService.Object);

            var currentUser = new User() { UserName = "Admin" };
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
