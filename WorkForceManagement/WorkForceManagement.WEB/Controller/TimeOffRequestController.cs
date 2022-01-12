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
        public async Task<ActionResult<List<TimeOffRequestResponseDTO>>> GetAllTimeOffRequests()
        {
            List<TimeOffRequestResponseDTO> timeOffRequestResponseModel =
                new List<TimeOffRequestResponseDTO>();
            var requests = await _timeOffRequestService.GetAllRequests();

            timeOffRequestResponseModel = _mapper.Map<List<TimeOffRequestResponseDTO>>(requests);
            return Ok(requests);
        }

        [HttpGet("{timeOffRequestId}")]
        public async Task<ActionResult<TimeOffRequestResponseDTO>> GetTimeOffRequest(Guid timeOffRequestId)
        {
            TimeOffRequest requestFromDB = await _timeOffRequestService.GetTimeOffRequest(timeOffRequestId);
            if (requestFromDB == null)
            {
                return NotFound("TimeOff Request doesn't exist.");
            }
            var requestModel = _mapper.Map<TimeOffRequestResponseDTO>(requestFromDB);
            return Ok(requestModel);
        }


        [HttpPost]
        public ActionResult CreateTimeOffRequest(TimeOffRequestRequestDTO timeOffRequestRequestModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            TimeOffRequest timeOffRequest = _mapper.Map<TimeOffRequest>(timeOffRequestRequestModel);
            timeOffRequest.Type = (TimeOffRequestType)timeOffRequestRequestModel.TimeOffRequestType;
            _timeOffRequestService.CreateTimeOffRequest(timeOffRequest);
            return Ok(timeOffRequest);
        }

        [HttpPut("{timeOffRequestId}")]
        public async Task<ActionResult> UpdateTimeOffRequest(Guid timeOffRequestId, TimeOffRequestRequestDTO timeOffRequestRequestDTO)
        {
            TimeOffRequest timeOffRequest = await _timeOffRequestService.GetTimeOffRequest(timeOffRequestId);
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }        
           timeOffRequest.ChangeDate = DateTime.Now;
         TimeOffRequest updatedRequest =  await _timeOffRequestService.UpdateTimeOffRequest(timeOffRequestId,
              (TimeOffRequestType)timeOffRequestRequestDTO.TimeOffRequestType, timeOffRequestRequestDTO.Description,
              timeOffRequestRequestDTO.StartDate,
              timeOffRequestRequestDTO.EndDate);
            return Ok(updatedRequest);
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
