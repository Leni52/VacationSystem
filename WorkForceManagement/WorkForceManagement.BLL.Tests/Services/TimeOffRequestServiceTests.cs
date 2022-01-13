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
 public class TimeOffRequestServiceTests
    {
        //create
        [Fact]
        public void Create_ValidRequest_Pass()
        {
            //arrange
            var requestRepositoryStub = new Mock<IRepository<TimeOffRequest>>();
            User currentUser = new User()
            {
                UserName = "admin",
            };

            var sut = new TimeOffRequestService(requestRepositoryStub.Object);

            TimeOffRequest timeOffRequest = new TimeOffRequest();
            //act
            var result = sut.CreateTimeOffRequest(timeOffRequest, currentUser.Id);

            Assert.NotNull(result);
            //asert
        }
        [Fact]
        public void Create_InvalidRequest_Fail()
        {
            //arrange
            var requestRepositoryStub = new Mock<IRepository<TimeOffRequest>>();
            User currentUser = new User()
            {
                UserName = "admin",
            };
           var currentUserId = "abc";
            var sut = new TimeOffRequestService(requestRepositoryStub.Object);

            TimeOffRequest timeOffRequest = new TimeOffRequest();
            //act
            var result = sut.CreateTimeOffRequest(timeOffRequest, currentUserId);

            Assert.Null(result);
            //asert
        }
        //delete
            [Fact]
        public void Delete_ValidId_Pass()
        {
            //arrange
            var requestRepositoryStub = new Mock<IRepository<TimeOffRequest>>();

            var sut = new TimeOffRequestService(requestRepositoryStub.Object);

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

            var sut = new TimeOffRequestService(requestRepositoryStub.Object);

            TimeOffRequest timeOffRequest = new TimeOffRequest();
            //act
            var result = sut.DeleteTimeOffRequest(Guid.NewGuid());

            Assert.NotNull(result);
            //asert
        }
        //get
        [Fact]
        public async void GetAllRequests_ValidRequest_Pass()
        {
            //arrange
            var requestRepositoryStub = new Mock<IRepository<TimeOffRequest>>();

            var sut = new TimeOffRequestService(requestRepositoryStub.Object);
           
            //act
            var result = sut.GetAllRequests();
            //asert
            Assert.IsType<List<TimeOffRequest>>(await sut.GetAllRequests());      
        }
        [Fact]
        public async void GetRequest_ValidRequest_Pass()
        {
            //arrange
            var requestRepositoryStub = new Mock<IRepository<TimeOffRequest>>();

            var sut = new TimeOffRequestService(requestRepositoryStub.Object);
            TimeOffRequest request = new TimeOffRequest();
            Guid reqId = request.Id;
            //act
            var result =await sut.GetTimeOffRequest(reqId);
            //asert
            Assert.IsType<TimeOffRequest>(result);               
        }
        
        [Fact]
        public async void GetRequest_InvalidRequest_Fail()
        {
            //arrange
            var requestRepositoryStub = new Mock<IRepository<TimeOffRequest>>();

            var sut = new TimeOffRequestService(requestRepositoryStub.Object);
            TimeOffRequest request = new TimeOffRequest();
            Guid reqId = request.Id;
            //act
            var result = await sut.GetTimeOffRequest(reqId);
            //asert
            Assert.IsNotType<Team>(result);
        }
    }
}
