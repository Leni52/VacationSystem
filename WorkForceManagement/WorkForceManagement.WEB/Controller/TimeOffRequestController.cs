using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkForceManagement.BLL.Exceptions;
using WorkForceManagement.BLL.Services;
using WorkForceManagement.DAL;
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
        private readonly IMailService _mailService;
        private readonly IMapper _mapper;
        public TimeOffRequestController(
            ITimeOffRequestService timeOffRequestService,
            IUserService userSerivce,
            IMailService mailService,
            IMapper mapper)
        {
            _timeOffRequestService = timeOffRequestService;
            _userService = userSerivce;
            _mailService = mailService;
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

            if(await _timeOffRequestService.CheckTimeOffRequest(timeOffRequest.Id) == "Approved")
            {
                List<User> usersToSendEmailTo = await _userService.GetUsersUnderTeamLeader(currentUser);

                if (usersToSendEmailTo.Count != 0)
                {
                    foreach (User u in usersToSendEmailTo)
                    {
                        await _mailService.SendEmail(new MailRequest()
                        {
                            ToEmail = u.Email,
                            Body = "TeamLeader OOO",
                            Subject = $"{currentUser.UserName} is OOO until {_timeOffRequestService.GetTimeOffRequest(timeOffRequest.Id).Result.EndDate}!"
                        });
                    }
                }
            }
            
            return Ok(timeOffRequestRequestDTO);
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
        public async  Task<ActionResult> DeleteTimeOffRequest(Guid timeOffRequestId)
        {
            if (!(Guid.Empty == timeOffRequestId))
            {
                await _timeOffRequestService.DeleteTimeOffRequest(timeOffRequestId);
                return NoContent();
            }
            return BadRequest();
        }

        [HttpGet("MyRequests")]
        [Authorize]
        public async Task<ActionResult<List<TimeOffRequest>>> GetMyRequests()
        {
            User currentUser = await _userService.GetCurrentUser(User);

            List<TimeOffRequest> myRequests = await _timeOffRequestService.GetMyRequests(currentUser.Id);
            
            var myRequestsDTO = _mapper.Map<List<TimeOffRequestResponseDTO>>(myRequests);
            
            return Ok(myRequestsDTO);
        }

        [HttpPatch("{timeOffRequestId}/AnswerTimeOffRequest/{isApproved:bool}")]
        //[Authorize(Policy = "TeamLeader")]
        public async Task<ActionResult> AnswerTimeOffRequest(Guid timeOffRequestId, bool isApproved)
        {
            User currentUser = await _userService.GetCurrentUser(User);

            await _timeOffRequestService.AnswerTimeOffRequest(timeOffRequestId, isApproved, currentUser);

            if(await _timeOffRequestService.CheckTimeOffRequest(timeOffRequestId) == "Approved")
            {
                List<User> usersToSendEmailTo = await _userService.GetUsersUnderTeamLeader(currentUser);

                if(usersToSendEmailTo.Count != 0)
                {
                    foreach (User u in usersToSendEmailTo)
                    {
                        await _mailService.SendEmail(new MailRequest()
                        {
                            ToEmail = u.Email,
                            Body = "TeamLeader OOO",
                            Subject = $"{currentUser.UserName} is OOO until {_timeOffRequestService.GetTimeOffRequest(timeOffRequestId).Result.EndDate}!"
                        });
                    }
                }
            }

            return Ok();
        }

        [HttpPatch("CheckTimeOffRequest/{timeOffRequestId}")]
        public async Task<ActionResult> CheckTimeOffRequest(Guid timeOffRequestId)
        {
            try
            {
                return Ok(await _timeOffRequestService.CheckTimeOffRequest(timeOffRequestId));
            }
            catch (ItemDoesNotExistException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
