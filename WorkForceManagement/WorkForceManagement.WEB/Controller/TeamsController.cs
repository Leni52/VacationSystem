using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkForceManagement.BLL.Services;
using WorkForceManagement.DAL.Entities;
using WorkForceManagement.DTO.RequestModels;
using WorkForceManagement.DTO.ResponseDTO;

namespace WorkForceManagement.WEB.Controller
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService _teamService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public TeamsController(
            IMapper mapper,
            ITeamService teamService,
            IUserService userService) : base()
        {
            _mapper = mapper;
            _teamService = teamService;
            _userService = userService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<TeamRequestDTO>> CreateTeam(TeamRequestDTO model)
        {
            var currentUser = await _userService.GetCurrentUser(User);
            var team = new Team();
            var teamLeader = await _userService.GetUserById(model.TeamLeaderId);

            _mapper.Map(model, team);
            team.TeamLeader = teamLeader;
            await _teamService.Create(team, currentUser);

            return Ok(model);
        }

        [HttpGet("{teamId}")]
        public async Task<ActionResult<TeamResponseDTO>> GetTeamById(Guid teamId)
        {
            var result = _mapper.Map<TeamResponseDTO>(await _teamService.GetTeamWithId(teamId));

            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<TeamResponseDTO>>> GetAllTeams()
        {
            var result = _mapper.Map<List<TeamResponseDTO>>(await _teamService.GetAllTeams());

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{teamId}")]
        public async Task<ActionResult> UpdateTeam(Guid teamId, TeamRequestDTO model)
        {
            var currentUser = await _userService.GetCurrentUser(User);
            var team = await _teamService.GetTeamWithId(teamId);
            var teamLeader = await _userService.GetUserById(model.TeamLeaderId);

            _mapper.Map(model, team);
            team.TeamLeader = teamLeader;

            await _teamService.UpdateTeam(team, teamId, currentUser);

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{teamId}")]
        public async Task<IActionResult> DeleteTeam(Guid teamId)
        {
            await _teamService.DeleteTeam(teamId);

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("UpdateTeamLeader/{teamId}&{newLeaderId}")]
        public async Task<IActionResult> UpdateTeamLeader(Guid teamId, Guid newLeaderId)
        {
            var currentUser = await _userService.GetCurrentUser(User);
            var user = await _userService.GetUserById(newLeaderId);

            await _teamService.UpdateTeamLeader(teamId, user, currentUser);

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AddUserToTeam/{teamId}&{userId}")]
        public async Task<IActionResult> AddUserToTeam(Guid teamId, Guid userId)
        {
            var currentUser = await _userService.GetCurrentUser(User);
            var user = await _userService.GetUserById(userId);

            await _teamService.AddUserToTeam(teamId, user, currentUser);

            return Ok();
        }

        [HttpGet("GetTeamMembers/{teamId}")]
        public async Task<ActionResult<List<UserResponseDTO>>> GetTeamMembers(Guid teamId)
        {
            var result = _mapper.Map<List<UserResponseDTO>>(await _teamService.GetAllTeamMembers(teamId));

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("RemoveUserFromTeam/{teamId}&{userId}")]
        public async Task<IActionResult> RemoveUserFromTeam(Guid teamId, Guid userId)
        {
            var currentUser = await _userService.GetCurrentUser(User);

            var user = await _userService.GetUserById(userId);

            await _teamService.RemoveUserFromTeam(teamId, user, currentUser);

            return Ok();
        }
    }
}