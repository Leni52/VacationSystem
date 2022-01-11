using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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

        [HttpGet]
        public async Task<ActionResult<List<TimeOffRequestResponseModel>>> GetAllTimeOffRequests()
        {
            List<TimeOffRequestResponseModel> timeOffRequestResponseModel =
                new List<TimeOffRequestResponseModel>();
            var requests = await _timeOffRequestService.GetAllRequests();

            timeOffRequestResponseModel = _mapper.Map<List<TimeOffRequestResponseModel>>(requests);
            return Ok(requests);
        }

        [HttpGet("{timeOffRequestId}")]
        public async Task<ActionResult<TimeOffRequestResponseModel>> GetTimeOffRequest(Guid timeOffRequestId)
        {
            TimeOffRequest requestFromDB = await _timeOffRequestService.GetTimeOffRequest(timeOffRequestId);
            if (requestFromDB == null)
            {
                return NotFound("TimeOff Request doesn't exist.");
            }
            var requestModel = _mapper.Map<TimeOffRequestResponseModel>(requestFromDB);
            return Ok(requestModel);
        }


        [HttpPost]
        public ActionResult CreateTimeOffRequest(TimeOffRequestRequestModel timeOffRequestRequestModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            TimeOffRequest timeOffRequest = _mapper.Map<TimeOffRequest>(timeOffRequestRequestModel);
            timeOffRequest.Type = (TimeOffRequestType)timeOffRequestRequestModel.TimeOffRequestType;
            _timeOffRequestService.CreateTimeOffRequest(timeOffRequest);
            return Ok();
        }

        [HttpPut("{timeOffRequestId}")]
        public async Task<ActionResult> UpdateTimeOffRequest(Guid timeOffRequestId, TimeOffRequestRequestModel timeOffRequestRequestModel)
        {
            TimeOffRequest timeOffRequest = await _timeOffRequestService.GetTimeOffRequest(timeOffRequestId);
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var tor = _mapper.Map<TimeOffRequest>(timeOffRequestRequestModel);
            timeOffRequest.Description = timeOffRequestRequestModel.Description;
            timeOffRequest.ChangeDate = DateTime.Now;
            timeOffRequest.Type = (TimeOffRequestType)timeOffRequestRequestModel.TimeOffRequestType;
            timeOffRequest.StartDate = timeOffRequestRequestModel.StartDate;
            timeOffRequest.EndDate = timeOffRequestRequestModel.EndDate;
          await _timeOffRequestService.UpdateTimeOffRequest(timeOffRequestId, timeOffRequest.Type);
            return Ok(timeOffRequest);
        }

        [HttpDelete("{timeOffRequestId}")]
        public ActionResult DeleteTimeOffRequest(Guid timeOffRequestId)
        {
            if (!(Guid.Empty == timeOffRequestId))
            {
                _timeOffRequestService.DeleteTimeOffRequest(timeOffRequestId);
                return Ok();
            }
            return BadRequest();
        }

    }
}
