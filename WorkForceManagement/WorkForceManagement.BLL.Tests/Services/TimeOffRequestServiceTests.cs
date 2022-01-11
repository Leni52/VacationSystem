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
        public async Task CreateAsync_ValidRequest_Passes()
        {
            //arrange
            var requestRepositoryStub = new Mock<IRepository<TimeOffRequest>>();
            

            var sut = new TimeOffRequestService(requestRepositoryStub.Object);

            TimeOffRequest timeOffRequest = new TimeOffRequest();
            //act
            var result = await Record.ExceptionAsync(() => sut.CreateTimeOffRequest(timeOffRequest));

            Assert.Null(result);
            //asert
        }
    }
}
