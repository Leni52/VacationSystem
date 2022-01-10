using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkForceManagement.BLL.Services;
using WorkForceManagement.DAL.Entities;
using WorkForceManagement.DTO.Requests;
using WorkForceManagement.DTO.Responses;

namespace WorkForceManagement.WEB.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeOffRequestController : ControllerBase
    {
        private readonly ITimeOffRequestService _timeOffRequestService;
        private readonly IMapper _mapper;
        public TimeOffRequestController(ITimeOffRequestService timeOffRequestService, 
            IMapper mapper)
        {
            _timeOffRequestService = timeOffRequestService;
            _mapper = mapper;
        }
        
        [HttpGet("GetAllTimeOffRequests")]
        public ActionResult<List<TimeOffRequestResponseModel>> GetAllTimeOffRequests()
        {
            List<TimeOffRequestResponseModel> timeOffRequestResponseModel =
                new List<TimeOffRequestResponseModel>();
            var requests = _timeOffRequestService.GetAllRequests();
            if (requests.Count == 0)
            {
                return NotFound("No requests in the database.");
            }
            timeOffRequestResponseModel = _mapper.Map<List<TimeOffRequestResponseModel>>(requests);
            return Ok(requests);
        }
       
        [HttpGet("{timeOffRequestId}")]
        public ActionResult<TimeOffRequestResponseModel> GetTimeOffRequest(Guid id)
        {
            TimeOffRequest requestFromDB = _timeOffRequestService.GetTimeOffRequest(id);
            if (requestFromDB == null)
            {
                return NotFound("TimeOff Request doesn't exist.");
            }
            var requestModel = _mapper.Map<TimeOffRequestResponseModel>(requestFromDB);
            return Ok(requestModel);
        }

        
        [HttpPost("CreateTimeOffRequest")]
        public ActionResult CreateTimeOffRequest(TimeOffRequestRequestModel timeOffRequestRequestModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            TimeOffRequest timeOffRequest = _mapper.Map<TimeOffRequest>(timeOffRequestRequestModel);
            _timeOffRequestService.CreateTimeOffRequestAsync(timeOffRequest);
            return Ok();
        }
        
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }


        [HttpDelete("{id}")]
        public ActionResult DeleteTimeOffRequest(Guid id)
        {
            if (!(id.Equals(null)))
            {
                _timeOffRequestService.DeleteTimeOffRequest(id);
                return Ok();
            }
            return BadRequest();
        }
    }
}
