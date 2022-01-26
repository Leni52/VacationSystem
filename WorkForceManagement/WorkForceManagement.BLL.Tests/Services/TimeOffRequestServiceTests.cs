using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkForceManagement.BLL.Exceptions;
using WorkForceManagement.BLL.Services;
using WorkForceManagement.BLL.Services.Interfaces;
using WorkForceManagement.DAL.Entities;
using WorkForceManagement.DAL.Repositories;
using Xunit;

namespace WorkForceManagement.BLL.Tests.Services
{
    public class TimeOffRequestServiceTests
    {
        private readonly Mock<IRepository<TimeOffRequest>> requestRepositoryStub = new();
        private readonly Mock<IUserService> userServiceMock = new();
        private readonly Mock<ITeamService> teamServiceMock = new();
        private readonly Mock<IMailService> mailService = new();
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
            User currentUser = new()
            {
                UserName = "admin",
            };

            TimeOffRequest timeOffRequest = new()
            {
                StartDate = DateTime.Now.Date.AddDays(1),
                EndDate = DateTime.Now.Date.AddDays(5),
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
            User currentUser = new() { UserName = "Admin" };
            TimeOffRequest t1 = new()
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
            TimeOffRequest timeOffRequest = new();
            //act
            var result = sut.DeleteTimeOffRequest(timeOffRequest.Id);

            Assert.NotNull(result);
            //asert
        }

        [Fact]
        public void Delete_InvalidId_Fail()
        {
            //arrange
            TimeOffRequest timeOffRequest = new();
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
            User currentUser = new()
            { UserName = "Admin" };

            //act
            var result = await sut.GetAllRequests();
            //asert
            Assert.IsType<List<TimeOffRequest>>(result);
        }
        [Fact]
        public async void GetMyRequests_ValidRequest_Pass()
        {
            //arrange
            requestRepositoryStub.Setup(reqRep => reqRep.All())
              .ReturnsAsync(new List<TimeOffRequest>());
            User currentUser = new()
            { UserName = "Admin" };

            //act
            var result = await sut.GetMyRequests(Guid.Parse(currentUser.Id));
            //asert
            Assert.IsType<List<TimeOffRequest>>(result);
        }
        [Fact]
        public async void GetRequest_ValidRequest_Pass()
        {
            //arrange
            User currentUser = new()
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
            var currentUser = new User() { UserName = "Admin", DaysOff = 20, CreatedTimeOffRequests = new List<TimeOffRequest>() };
            userServiceMock.Setup(user => user.GetUserById(It.IsAny<Guid>()))
                .ReturnsAsync(currentUser);
            requestRepositoryStub.Setup(reqRep => reqRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync(new TimeOffRequest());
            //act
            var timeOffRequest = new TimeOffRequest()
            {
                StartDate = DateTime.Now.Date.AddDays(1),
                EndDate = DateTime.Now.Date.AddDays(5)
            };

            var result = await Record.ExceptionAsync(() => sut.UpdateTimeOffRequest(new Guid(),
                timeOffRequest, currentUser.Id));
            //assert
            Assert.Null(result);
        }

        [Fact]
        public async void GetMyColleguesTimeOffRequests_ValidRequest_ReturnsListOfUsers()
        {
            //arrange
            List<Team> teams = new();
            User currentUser = new() { Teams = teams };

            userServiceMock.Setup(userService => userService.GetUserTeams(It.IsAny<User>()))
                .Returns(teams);
            //act
            //assert
            Assert.IsType<List<User>>(await sut.GetMyColleguesTimeOffRequests(currentUser));
        }

        [Fact]
        public async Task AnswerTimeOffRequest_ValidRequest_Passes()
        {
            //arrange
            User requester = new User() { Id = Guid.NewGuid().ToString(), Email = "test@gmail.com" };
            User currentUser = new User() { Id = Guid.NewGuid().ToString(), TimeOffRequestsApproved = new List<TimeOffRequest>(), TimeOffRequestsToApprove = new List<TimeOffRequest>() };
            List<User> approvers = new List<User>();
            approvers.Add(currentUser);

            Guid timeOffRequestId = Guid.NewGuid();
            TimeOffRequest torRequest = new TimeOffRequest()
            {
                Requester = requester,
                Id = timeOffRequestId,
                Approvers = approvers
            };

            requestRepositoryStub.Setup(reqRep => reqRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync(torRequest);
            userServiceMock.Setup(service => service.GetUsersUnderTeamLeader(It.IsAny<User>()))
                .ReturnsAsync(new List<User>());

            //act
            var exception = await Record.ExceptionAsync(() => sut.AnswerTimeOffRequest(timeOffRequestId, true, currentUser, "reason"));
            //assert
            Assert.Null(exception);
        }

        [Fact]
        public async Task AnswerTimeOffRequest_RequestClosed_ThrowsException()
        {
            //arrange
            User requester = new User() { Id = Guid.NewGuid().ToString(), Email = "test@gmail.com" };
            User currentUser = new User() { Id = Guid.NewGuid().ToString(), TimeOffRequestsApproved = new List<TimeOffRequest>(), TimeOffRequestsToApprove = new List<TimeOffRequest>() };
            List<User> approvers = new List<User>();
            approvers.Add(currentUser);

            Guid timeOffRequestId = Guid.NewGuid();
            TimeOffRequest torRequest = new TimeOffRequest()
            {
                Requester = requester,
                Id = timeOffRequestId,
                Approvers = approvers,
                Status = TimeOffRequestStatus.Rejected
            };

            requestRepositoryStub.Setup(reqRep => reqRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync(torRequest);
            userServiceMock.Setup(service => service.GetUsersUnderTeamLeader(It.IsAny<User>()))
                .ReturnsAsync(new List<User>());

            //act
            await Assert.ThrowsAsync<TimeOffRequestIsClosedException>(() => sut.AnswerTimeOffRequest(timeOffRequestId, true, currentUser, "reason"));
        }

        [Fact]
        public async Task AnswerTimeOffRequest_UserIsntApproverException_ThrowsException()
        {
            //arrange
            User requester = new User() { Id = Guid.NewGuid().ToString(), Email = "test@gmail.com" };
            User currentUser = new User() { Id = Guid.NewGuid().ToString(), TimeOffRequestsApproved = new List<TimeOffRequest>(), TimeOffRequestsToApprove = new List<TimeOffRequest>() };
            List<User> approvers = new List<User>();

            Guid timeOffRequestId = Guid.NewGuid();
            TimeOffRequest torRequest = new TimeOffRequest()
            {
                Requester = requester,
                Id = timeOffRequestId,
                Approvers = approvers
            };

            requestRepositoryStub.Setup(reqRep => reqRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync(torRequest);
            userServiceMock.Setup(service => service.GetUsersUnderTeamLeader(It.IsAny<User>()))
                .ReturnsAsync(new List<User>());

            //act
            await Assert.ThrowsAsync<UserIsntApproverException>(() => sut.AnswerTimeOffRequest(timeOffRequestId, true, currentUser, "reason"));
        }

        [Fact]
        public async Task Create_InvalidRequest_EndDateBeforeStartDate_Fail()
        {
            //arrange           
            User currentUser = new User()
            {
                UserName = "admin",
            };
            TimeOffRequest timeOffRequest = new TimeOffRequest()
            {
                StartDate = DateTime.Now.Date.AddDays(5),
                EndDate = DateTime.Now.Date.AddDays(1),
                Description = "Testing and testing",
                Type = TimeOffRequestType.Paid
            };
            //act
            //asert
            await Assert.ThrowsAsync<InvalidDatesException>(() => sut.CreateTimeOffRequest(timeOffRequest, currentUser));

        }
        [Fact]
        public async Task CheckForDaysOff_NotEnoughDaysOff_Fail()
        {
            //arrange
            User currentUser = new User()
            {
                UserName = "admin",
                DaysOff = 3
            };
            TimeOffRequest timeOffRequest = new TimeOffRequest()
            {
                StartDate = new DateTime(2022, 1, 17),
                EndDate = new DateTime(2022, 1, 21),
                Description = "Testing and testing",
                Type = TimeOffRequestType.Paid
            };
            //act    
            //assert
            await Assert.ThrowsAsync<InvalidDatesException>(() => sut.CreateTimeOffRequest(timeOffRequest, currentUser));
        }
        [Fact]
        public async Task CheckForDaysOff_IncludesSickLeave_Pass()
        {
            //arrange
            User currentUser = new User()
            {
                UserName = "admin",
                DaysOff = 10,
                CreatedTimeOffRequests = new List<TimeOffRequest>(),
                Teams = new List<Team>()
            };

            TimeOffRequest timeOffRequest = new TimeOffRequest()
            {
                Requester = currentUser,
                StartDate = DateTime.Now.Date.AddDays(1),
                EndDate = DateTime.Now.Date.AddDays(5),
                Description = "Testing and testing",
                Type = TimeOffRequestType.SickLeave,
                Status = TimeOffRequestStatus.Created
            };
            //act    
            requestRepositoryStub.Setup(torRep => torRep.Get(It.IsAny<Guid>()))
                 .ReturnsAsync(timeOffRequest);
            userServiceMock.Setup(service => service.GetUsersUnderTeamLeader(It.IsAny<User>()))
                .ReturnsAsync(new List<User>());
            await sut.CreateTimeOffRequest(timeOffRequest, currentUser);
            //assert
            Assert.Equal(TimeOffRequestStatus.Approved, timeOffRequest.Status);
        }

        [Fact]
        public async void CheckForDaysOff_IncludesWeekend()
        {
            //arrange
            User currentUser = new User()
            {
                UserName = "admin",
                DaysOff = 9,
                CreatedTimeOffRequests = new List<TimeOffRequest>(),
                Teams = new List<Team>()
            };
            TimeOffRequest timeOffRequest = new TimeOffRequest()
            {
                StartDate = DateTime.Now.Date.AddDays(1),
                EndDate = DateTime.Now.Date.AddDays(10),
                Description = "Testing and testing",
                Type = TimeOffRequestType.Paid,
                Status = TimeOffRequestStatus.Created,
                Requester = currentUser
            };
            requestRepositoryStub.Setup(torRep => torRep.Get(It.IsAny<Guid>()))
               .ReturnsAsync(timeOffRequest);
            userServiceMock.Setup(service => service.GetUsersUnderTeamLeader(It.IsAny<User>()))
.ReturnsAsync(new List<User>());
            var result = await Record.ExceptionAsync(() => sut.CreateTimeOffRequest(timeOffRequest, currentUser));
            Assert.Null(result);
        }
        [Fact]
        public async void CheckForDaysOff_IncludesHoliday()
        {
            //arrange
            User currentUser = new User()
            {
                UserName = "admin",
                DaysOff = 1,
                CreatedTimeOffRequests = new List<TimeOffRequest>(),
                Teams = new List<Team>()
            };
            TimeOffRequest timeOffRequest = new TimeOffRequest()
            {
                StartDate = new DateTime(2022, 3, 3),
                EndDate = new DateTime(2022, 3, 4),
                Description = "Testing and testing",
                Type = TimeOffRequestType.Paid,
                Status = TimeOffRequestStatus.Created,
                Requester = currentUser
            };
            requestRepositoryStub.Setup(torRep => torRep.Get(It.IsAny<Guid>()))
               .ReturnsAsync(timeOffRequest);
            userServiceMock.Setup(service => service.GetUsersUnderTeamLeader(It.IsAny<User>()))
.ReturnsAsync(new List<User>());
            var result = await Record.ExceptionAsync(() => sut.CreateTimeOffRequest(timeOffRequest, currentUser));
            Assert.Null(result);
        }
        [Fact]
        public async void CheckForDaysOff_IncludesWeekendAndHoliday()
        {
            //arrange
            User currentUser = new User()
            {
                UserName = "admin",
                DaysOff = 1,
                CreatedTimeOffRequests = new List<TimeOffRequest>(),
                Teams = new List<Team>()
            };
            TimeOffRequest timeOffRequest = new()
            {
                StartDate = new DateTime(2022, 3, 3),
                EndDate = new DateTime(2022, 3, 6),
                Description = "Testing and testing",
                Type = TimeOffRequestType.Paid,
                Status = TimeOffRequestStatus.Created,
                Requester = currentUser
            };
            requestRepositoryStub.Setup(torRep => torRep.Get(It.IsAny<Guid>()))
               .ReturnsAsync(timeOffRequest);
            userServiceMock.Setup(service => service.GetUsersUnderTeamLeader(It.IsAny<User>()))
.ReturnsAsync(new List<User>());
            var result = await Record.ExceptionAsync(() => sut.CreateTimeOffRequest(timeOffRequest, currentUser));
            Assert.Null(result);
        }
        [Fact]
        public async Task CheckForDaysOff_AwaitingStatus_ExecutesSuccessfully()
        {
            //arrange
            User requester = new()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "requester"
            };
            TimeOffRequest timeOffRequest = new TimeOffRequest()
            {
                Requester = requester,
                StartDate = DateTime.Today.AddDays(5),
                EndDate = DateTime.Today.AddDays(10),
                Description = "Testing valid cancellation",
                Type = TimeOffRequestType.Paid,
                Status = TimeOffRequestStatus.Awaiting
            };
            requestRepositoryStub.Setup(torRep => torRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync(timeOffRequest);

            //act
            await sut.CancelTimeOffRequest(Guid.NewGuid());

            Assert.Equal(TimeOffRequestStatus.Cancelled, timeOffRequest.Status);
            //asert
        }

        [Fact]
        public async Task CheckForDaysOff_CreatedStatus_ExecutesSuccessfully()
        {
            //arrange
            User requester = new()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "requester"
            };
            TimeOffRequest timeOffRequest = new()
            {
                Requester = requester,
                StartDate = DateTime.Today.AddDays(5),
                EndDate = DateTime.Today.AddDays(10),
                Description = "Testing valid cancellation",
                Type = TimeOffRequestType.Paid,
                Status = TimeOffRequestStatus.Created
            };
            requestRepositoryStub.Setup(torRep => torRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync(timeOffRequest);

            //act
            await sut.CancelTimeOffRequest(Guid.NewGuid());

            Assert.Equal(TimeOffRequestStatus.Cancelled, timeOffRequest.Status);
            //asert
        }

        [Fact]
        public async Task CheckForDaysOff_NullTORSupplied_Fails()
        {
            //arrange
            //act    
            requestRepositoryStub.Setup(torRep => torRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync((TimeOffRequest)null);

            //assert
            await Assert.ThrowsAsync<ItemDoesNotExistException>(() => sut.CancelTimeOffRequest(Guid.NewGuid()));
        }

        [Fact]
        public async Task CheckForDaysOff_OneDayBeforeStartDate_Fails()
        {
            //arrange
            User requester = new()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "requester"
            };
            TimeOffRequest timeOffRequest = new()
            {
                Requester = requester,
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(10),
                Description = "Testing valid cancellation",
                Type = TimeOffRequestType.Unpaid,
                Status = TimeOffRequestStatus.Awaiting
            };
            requestRepositoryStub.Setup(torRep => torRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync(timeOffRequest);

            //act
            //assert
            await Assert.ThrowsAsync<CannotCancelTimeOffRequestException>(
                () => sut.CancelTimeOffRequest(Guid.NewGuid()));
        }

        [Fact]
        public async Task CheckForDaysOff_TwoDaysBeforeStartDate_Fails()
        {
            //arrange
            User requester = new()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "requester"
            };
            TimeOffRequest timeOffRequest = new()
            {
                Requester = requester,
                StartDate = DateTime.Today.AddDays(2),
                EndDate = DateTime.Today.AddDays(10),
                Description = "Testing valid cancellation",
                Type = TimeOffRequestType.Unpaid,
                Status = TimeOffRequestStatus.Awaiting
            };
            requestRepositoryStub.Setup(torRep => torRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync(timeOffRequest);

            //act
            //assert
            await Assert.ThrowsAsync<CannotCancelTimeOffRequestException>(() => sut.CancelTimeOffRequest(Guid.NewGuid()));
        }
        [Fact]
        public async Task CheckForDaysOff_ThreeDaysBeforeStartDate_Fails()
        {
            //arrange
            User requester = new()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "requester"
            };
            TimeOffRequest timeOffRequest = new TimeOffRequest()
            {
                Requester = requester,
                StartDate = DateTime.Today.AddDays(3),
                EndDate = DateTime.Today.AddDays(10),
                Description = "Testing valid cancellation",
                Type = TimeOffRequestType.Unpaid,
                Status = TimeOffRequestStatus.Awaiting
            };
            requestRepositoryStub.Setup(torRep => torRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync(timeOffRequest);

            //act
            //assert
            await Assert.ThrowsAsync<CannotCancelTimeOffRequestException>(() => sut.CancelTimeOffRequest(Guid.NewGuid()));
        }

        [Fact]
        public async Task CheckForDaysOff_TORAlreadyApproved_Fails()
        {
            //arrange
            User requester = new User()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "requester"
            };
            TimeOffRequest timeOffRequest = new TimeOffRequest()
            {
                Requester = requester,
                StartDate = DateTime.Today.AddDays(4),
                EndDate = DateTime.Today.AddDays(10),
                Description = "Testing valid cancellation",
                Type = TimeOffRequestType.Unpaid,
                Status = TimeOffRequestStatus.Approved
            };
            requestRepositoryStub.Setup(torRep => torRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync(timeOffRequest);

            //act
            //assert
            await Assert.ThrowsAsync<CannotCancelTimeOffRequestException>(() => sut.CancelTimeOffRequest(Guid.NewGuid()));
        }

        [Fact]
        public async Task CheckForDaysOff_TORRejected_Fails()
        {
            //arrange
            User requester = new User()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "requester"
            };
            TimeOffRequest timeOffRequest = new TimeOffRequest()
            {
                Requester = requester,
                StartDate = DateTime.Today.AddDays(6),
                EndDate = DateTime.Today.AddDays(10),
                Description = "Testing valid cancellation",
                Type = TimeOffRequestType.Unpaid,
                Status = TimeOffRequestStatus.Rejected
            };
            requestRepositoryStub.Setup(torRep => torRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync(timeOffRequest);

            //act
            //assert
            await Assert.ThrowsAsync<CannotCancelTimeOffRequestException>(() => sut.CancelTimeOffRequest(Guid.NewGuid()));
        }

        [Fact]
        public async Task SaveFile_ValidFileSupplied_Passes()
        {
            TimeOffRequest timeOffRequest = new();
            requestRepositoryStub.Setup(torRep => torRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync(timeOffRequest);
            TblFile file = new();

            await sut.SaveFile(file, Guid.NewGuid());
        }

        [Fact]
        public async Task SaveFile_InvalidTORIdSupplied_Fails()
        {
            requestRepositoryStub.Setup(torRep => torRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync((TimeOffRequest)null);
            TblFile file = new();
            await Assert.ThrowsAsync<ItemDoesNotExistException>(() => sut.SaveFile(file, Guid.NewGuid()));
        }

        [Fact]
        public async Task SaveFile_NullFileSupplied_Fails()
        {
            TimeOffRequest timeOffRequest = new();
            requestRepositoryStub.Setup(torRep => torRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync(timeOffRequest);
            TblFile file = null;
            await Assert.ThrowsAsync<ItemDoesNotExistException>(() => sut.SaveFile(file, Guid.NewGuid()));
        }
        [Fact]
        public async Task SaveFile_BothParamsNull_Fails()
        {
            requestRepositoryStub.Setup(torRep => torRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync((TimeOffRequest)null);
            TblFile file = null;
            await Assert.ThrowsAsync<ItemDoesNotExistException>(() => sut.SaveFile(file, Guid.NewGuid()));
        }
        [Fact]
        public async Task GetFile_ValidTORId_Passes()
        {
            TblFile file = new();
            TimeOffRequest timeOffRequest = new()
            {
                Pdf = file
            };
            requestRepositoryStub.Setup(torRep => torRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync(timeOffRequest);
            await sut.GetFile(Guid.NewGuid());
        }
        [Fact]
        public async Task GetFile_InvalidTORId_Fails()
        {
            requestRepositoryStub.Setup(torRep => torRep.Get(It.IsAny<Guid>()))
                .ReturnsAsync((TimeOffRequest)null);
            await Assert.ThrowsAsync<ItemDoesNotExistException>(() => sut.GetFile(Guid.NewGuid()));
        }
    }
}
