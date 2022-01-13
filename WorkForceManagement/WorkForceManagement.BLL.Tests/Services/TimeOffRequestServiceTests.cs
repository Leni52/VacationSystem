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
            var result = sut.CreateTimeOffRequest(timeOffRequest, currentUser);

            Assert.NotNull(result);
            //asert
        }
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
        
    }
}
