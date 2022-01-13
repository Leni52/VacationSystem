using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public TimeOffRequestController(ITimeOffRequestService timeOffRequestService,
           IUserService userService, IMapper mapper)
        {
            _timeOffRequestService = timeOffRequestService;
            _userService = userService;
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
            var requestModel = _mapper.Map<TimeOffRequestResponseDTO>(requestFromDB);
            return Ok(requestModel);
        }


        [HttpPost]
        public async Task<ActionResult> CreateTimeOffRequestAsync(TimeOffRequestRequestDTO timeOffRequestRequestDTO)
        {
            User currentUser = await _userService.GetCurrentUser(User);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            TimeOffRequest timeOffRequest = _mapper.Map<TimeOffRequest>(timeOffRequestRequestDTO);
            timeOffRequest.Type = (TimeOffRequestType)timeOffRequestRequestDTO.TimeOffRequestType;
            await _timeOffRequestService.CreateTimeOffRequest(timeOffRequest, currentUser.Id);
            return Ok(timeOffRequest);
        }

        [HttpPut("{timeOffRequestId}")]
        [Authorize(Policy = "TimeOffRequestCreator")]
        public async Task<ActionResult> UpdateTimeOffRequest(Guid timeOffRequestId, TimeOffRequestRequestDTO request)
        {
            User currentUser = await _userService.GetCurrentUser(User);
            TimeOffRequest timeOffRequest = await _timeOffRequestService.GetTimeOffRequest(timeOffRequestId);
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            _mapper.Map(request, timeOffRequest);
            var updatedRequest = await _timeOffRequestService.UpdateTimeOffRequest(timeOffRequestId, timeOffRequest, currentUser.Id);
            return Ok(updatedRequest);
        }

        [HttpDelete("{timeOffRequestId}")]
        [Authorize(Policy = "TimeOffRequestCreator")]
        public async Task<ActionResult> DeleteTimeOffRequest(Guid timeOffRequestId)
        {
            User currentUser = await _userService.GetCurrentUser(User);
            if (!(Guid.Empty == timeOffRequestId))
            {
                await _timeOffRequestService.DeleteTimeOffRequest(timeOffRequestId);
                return NoContent();
            }
            return BadRequest();
        }
        [HttpGet("MyRequests")]
        public async Task<ActionResult<List<TimeOffRequest>>> GetMyRequests()
        {
            User currentUser = await _userService.GetCurrentUser(User);
            List<TimeOffRequest> myRequests = new List<TimeOffRequest>();
            myRequests = await _timeOffRequestService.GetMyRequests(currentUser.Id);
            var myRequestsDTO = _mapper.Map<List<TimeOffRequestRequestDTO>>(myRequests);
            return Ok(myRequestsDTO);
        }
    }
}
