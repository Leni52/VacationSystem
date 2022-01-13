using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkForceManagement.BLL.Exceptions;
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
        public TimeOffRequestController(
            ITimeOffRequestService timeOffRequestService,
            IUserService userSerivce,
            IMapper mapper)
        {
            _timeOffRequestService = timeOffRequestService;
            _userService = userSerivce;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<TimeOffRequestResponseDTO>>> GetAllTimeOffRequests()
        {
            List<TimeOffRequestResponseDTO> timeOffRequestResponseModel =
                new List<TimeOffRequestResponseDTO>();
            var requests = await _timeOffRequestService.GetAllRequests();

            timeOffRequestResponseModel = _mapper.Map<List<TimeOffRequestResponseDTO>>(requests);
            return Ok(timeOffRequestResponseModel);
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
        public async Task<ActionResult> CreateTimeOffRequest(TimeOffRequestRequestDTO timeOffRequestRequestDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            User currentUser = await _userService.GetCurrentUser(User);
            TimeOffRequest timeOffRequest = _mapper.Map<TimeOffRequest>(timeOffRequestRequestDTO);

            timeOffRequest.Type = (TimeOffRequestType)timeOffRequestRequestDTO.TimeOffRequestType;
            await _timeOffRequestService.CreateTimeOffRequest(timeOffRequest, currentUser);

            return Ok();
        }

        [HttpPut("{timeOffRequestId}")]
        public async Task<ActionResult> UpdateTimeOffRequest(Guid timeOffRequestId, TimeOffRequestRequestDTO request)
        {
            TimeOffRequest timeOffRequest = await _timeOffRequestService.GetTimeOffRequest(timeOffRequestId);
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            _mapper.Map(request, timeOffRequest);
            var updatedRequest = await _timeOffRequestService.UpdateTimeOffRequest(timeOffRequestId, timeOffRequest);
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

        [HttpPatch("RejectTimeOffRequest/{timeOffRequestId}")]
        public async Task<ActionResult> RejectTimeOffRequest(Guid timeOffRequestId)
        {
            User currentUser = await _userService.GetCurrentUser(User);

            await _timeOffRequestService.RejectTimeOffRequest(timeOffRequestId, currentUser);
            //await _timeOffRequestService
            return Ok();
        }

        [HttpPatch("CheckTimeOffRequest/{timeOffRequestId}")]
        public async Task<ActionResult> CheckTimeOffRequest(Guid timeOffRequestId)
        {
            try
            {
                return Ok(await _timeOffRequestService.CheckTimeOffRequest(timeOffRequestId));
            }
            catch(ItemDoesNotExistException ex)
            {
                return NotFound(ex.Message);
            }                    
        }
    }
}
