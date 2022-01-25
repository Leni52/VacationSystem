using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WorkForceManagement.BLL.Exceptions;
using WorkForceManagement.BLL.Services.Interfaces;
using WorkForceManagement.DAL.Entities;
using WorkForceManagement.DTO.Requests;
using WorkForceManagement.DTO.ResponseDTO;
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
            IUserService userService,
            IMapper mapper)
        {
            _timeOffRequestService = timeOffRequestService;
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<TimeOffRequestResponseDTO>>> GetAllTimeOffRequests()
        {
            var results = _mapper.Map<List<TimeOffRequestResponseDTO>>(await _timeOffRequestService.GetAllRequests());

            return Ok(results);
        }

        [HttpGet("{timeOffRequestId}")]
        public async Task<ActionResult<TimeOffRequestResponseDTO>> GetTimeOffRequest(Guid timeOffRequestId)
        {
            TimeOffRequest requestFromDB = await _timeOffRequestService.GetTimeOffRequest(timeOffRequestId);

            var requestModel = _mapper.Map<TimeOffRequestResponseDTO>(requestFromDB);

            return Ok(requestModel);
        }

        [HttpPost]
        public async Task<ActionResult<TimeOffRequestRequestDTO>> CreateTimeOffRequest(TimeOffRequestRequestDTO timeOffRequestRequestDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            User currentUser = await _userService.GetCurrentUser(User);
            TimeOffRequest timeOffRequest = _mapper.Map<TimeOffRequest>(timeOffRequestRequestDTO);

            timeOffRequest.Type = (TimeOffRequestType)timeOffRequestRequestDTO.Type;
            await _timeOffRequestService.CreateTimeOffRequest(timeOffRequest, currentUser);
            return Ok(timeOffRequestRequestDTO);
        }

        [HttpPut("{timeOffRequestId}")]
        [Authorize(Policy = "TimeOffRequestCreator")]
        public async Task<ActionResult> UpdateTimeOffRequest(Guid timeOffRequestId, TimeOffRequestRequestDTO request)
        {
            User currentUser = await _userService.GetCurrentUser(User);
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            // _mapper.Map(request, timeOffRequest);
            TimeOffRequest timeOffRequest = _mapper.Map<TimeOffRequest>(request);
            await _timeOffRequestService.UpdateTimeOffRequest(timeOffRequestId, timeOffRequest, currentUser.Id);
            return Ok();
        }

        [HttpDelete("{timeOffRequestId}")]
        [Authorize(Policy = "TimeOffRequestCreator")]
        public async Task<ActionResult> DeleteTimeOffRequest(Guid timeOffRequestId)
        {
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
            var user = await _userService.GetCurrentUser(User);
            if (user != null)
            {
                var results = _mapper.Map<List<TimeOffRequestResponseDTO>>(await _timeOffRequestService.GetMyRequests(Guid.Parse(user.Id)));

                return Ok(results);
            }
            return BadRequest();
        }

        [HttpPatch("{timeOffRequestId}/AnswerTimeOffRequest/{isApproved}")]
        [Authorize(Policy = "TeamLeader")]
        public async Task<ActionResult> AnswerTimeOffRequest(Guid timeOffRequestId, bool isApproved, string reason)
        {
            var user = await _userService.GetCurrentUser(User);

            await _timeOffRequestService.AnswerTimeOffRequest(timeOffRequestId, isApproved, user, reason);

            return Ok();
        }

        [HttpGet("CheckTimeOffRequest/{timeOffRequestId}")]
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

        [HttpGet("RequestsWaitingForApproval")]
        [Authorize]
        public async Task<ActionResult<List<TimeOffRequest>>> RequestsWaitingForApproval()
        {
            var user = await _userService.GetCurrentUser(User);

            if (user != null)
            {
                var results = _mapper.Map<List<TimeOffRequestResponseDTO>>(user.TimeOffRequestsToApprove);

                return Ok(results);
            }

            return BadRequest();
        }
        [HttpGet("MyColleguesVacation")]
        public async Task<ActionResult> GetMyColleguesVacationList()
        {
            var user = await _userService.GetCurrentUser(User);
            
            if (user != null)
            {
                var members = await _timeOffRequestService.GetMyColleguesTimeOffRequests(user);
                
                var results = _mapper.Map<List<UserResponseDTO>>(members);
                
                return Ok(results);
            }

            return BadRequest();
        }

        [HttpPatch("CancelTimeOffRequest/{timeOffRequestId}")]
        [Authorize(Policy = "TimeOffRequestCreator")]
        public async Task<ActionResult> CancelTimeOffRequest(Guid timeOffRequestId)
        {
            await _timeOffRequestService.CancelTimeOffRequest(timeOffRequestId);

            return Ok();
        }
        
        [HttpPost("Add a file")]
        public async Task<IActionResult> IndexAsync(IFormFile postedFile, Guid TimeOffRequestId)
        {
            byte[] bytes;
            using (BinaryReader br = new BinaryReader(postedFile.OpenReadStream()))
            {
                bytes = br.ReadBytes((int)postedFile.Length);
            }
            TblFile Pdf = new()
            {
                Name = postedFile.FileName,
                ContentType = postedFile.ContentType,
                Data = bytes
            };
            await _timeOffRequestService.SaveFile(Pdf, TimeOffRequestId);
            return Ok(postedFile.FileName);
        }

        [HttpGet("Get TOR file")]
        public async Task<FileResult> DownloadFile(Guid TimeOffRequestId)
        {
            TblFile file = await _timeOffRequestService.GetFile(TimeOffRequestId);
            return File(file.Data, file.ContentType, file.Name);
        }
    }
}