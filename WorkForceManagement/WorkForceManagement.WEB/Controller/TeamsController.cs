using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkForceManagement.BLL.Services;
using WorkForceManagement.DAL.Entities;
using WorkForceManagement.DTO.RequestModels;
using WorkForceManagement.DTO.ResponseDTO;
using WorkForceManagement.DTO.ResponseModels;

namespace WorkForceManagement.WEB.Controller
{
    [Route("api/[controller]")]
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
            Team teamToAdd = new Team();
            User teamLeader = await _userService.GetUserById(model.TeamLeaderId);

            _mapper.Map(model, teamToAdd);
            teamToAdd.TeamLeader = teamLeader;

            await _teamService.Create(teamToAdd);

            return Ok(model);
        }

        [HttpGet("{teamId}")]
        public async Task<ActionResult<TeamResponseDTO>> GetTeamById(Guid teamId)
        {
            Team team = await _teamService.GetTeamWithId(teamId);

            var result = _mapper.Map<TeamResponseDTO>(team);

            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<TeamResponseDTO>>> GetAllTeams()
        {
            List<Team> teams = await _teamService.GetAllTeams();
            
            var result = _mapper.Map<List<TeamResponseDTO>>(teams);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{teamId}")]
        public async Task<ActionResult> UpdateTeam(Guid teamId, TeamRequestDTO model)
        {
            Team updatedTeam = new Team();
            User teamLeader = await _userService.GetUserById(model.TeamLeaderId);

            _mapper.Map(model, updatedTeam);
            updatedTeam.TeamLeader = teamLeader;

            await _teamService.UpdateTeam(updatedTeam, teamId);

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{teamId}")]
        public async Task<IActionResult> DeleteTeam(Guid teamId)
        {
            await _teamService.DeleteTeam(teamId);

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("UpdateTeamLeader/{teamId}&{newLeaderId}")]
        public async Task<IActionResult> UpdateTeamLeader(Guid teamId, Guid newLeaderId)
        {
            User newLeader = await _userService.GetUserById(newLeaderId);

            await _teamService.UpdateTeamLeader(teamId, newLeader);

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AddUserToTeam/{teamId}&{userId}")]
        public async Task<IActionResult> AddUserToTeam(Guid teamId, Guid userId)
        {
            User userToAdd = await _userService.GetUserById(userId);

            await _teamService.AddUserToTeam(teamId, userToAdd);

            return Ok();
        }

        [HttpGet("GetTeamMembers/{teamId}")]
        public async Task<ActionResult<List<UserResponseDTO>>> GetTeamMembers(Guid teamId)
        {
            List<User> teamMembers = await _teamService.GetAllTeamMembers(teamId);

            var result = _mapper.Map<List<UserResponseDTO>>(teamMembers);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("RemoveUserFromTeam/{teamId}&{userId}")]
        public  async Task<IActionResult> RemoveUserFromTeam(Guid teamId, Guid userId)
        {
            User userToDelete = await _userService.GetUserById(userId);

            await _teamService.RemoveUserFromTeam(teamId, userToDelete);

            return Ok();
        }



    }
}
