using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkForceManagement.BLL.Services;
using WorkForceManagement.DAL.Entities;
using WorkForceManagement.DTO.RequestModels;
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
            IMapper _mapper,
            ITeamService _teamService,
            IUserService _userService) : base()
        {
            this._mapper = _mapper;
            this._teamService = _teamService;
            this._userService = _userService;
        }

        [HttpPost("CreateTeam")]
        public async Task<ActionResult<TeamRequestDTO>> Post(TeamRequestDTO model)
        {
            Team teamToAdd = new Team();
            User teamLeader = await _userService.GetUserWithIdAsync(model.TeamLeaderId);

            _mapper.Map(model, teamToAdd);
            teamToAdd.TeamLeader = teamLeader;

            await _teamService.CreateAsync(teamToAdd);

            return model;
        }
        [HttpGet("GetTeamById/{teamId}")]
        public async Task<ActionResult<TeamResponseDTO>> GetTeamById(string teamId)
        {
            Team team = await _teamService.GetTeamWithIdAsync(teamId);

            var model = _mapper.Map<TeamResponseDTO>(team);

            return model;
        }

        [HttpGet("GetAllTeams")]
        public async Task<ActionResult<List<TeamResponseDTO>>> GetAllTeams()
        {
            List<Team> teams = await _teamService.GetAllTeamsAsync();
            var models = _mapper.Map<List<TeamResponseDTO>>(teams);


            return models;
        }

        [HttpPatch("UpdateTeam/{teamId}")]
        public async Task<ActionResult> UpdateTeam(string teamId, TeamRequestDTO model)
        {
            Team updatedTeam = new Team();
            _mapper.Map(model, updatedTeam);

            await _teamService.UpdateTeamAsync(updatedTeam, teamId);

            return Ok();
        }

        [HttpDelete("DeleteTeam/{teamId}")]
        public async Task<IActionResult> DeleteTeam(string teamId)
        {
            await _teamService.DeleteTeamAsync(teamId);

            return Ok();
        }

        [HttpPatch("UpdateTeamLeader/{teamId}&{newLeaderId}")]
        public async Task<IActionResult> UpdateTeamLeader(string teamId, string newLeaderId)
        {
            User newLeader = await _userService.GetUserWithIdAsync(newLeaderId);

            await _teamService.UpdateTeamLeaderAsync(teamId, newLeader);

            return Ok();
        }

        [HttpPost("AddUserToTeam/{teamId}&{userId}")]
        public async Task<IActionResult> AddUserToTeam(string teamId, string userId)
        {
            User userToAdd = await _userService.GetUserWithIdAsync(userId);

            await _teamService.AddUserToTeamAsync(teamId, userToAdd);

            return Ok();
        }

        [HttpGet("GetTeamMembers/{teamId}")]
        public async Task<ActionResult<List<UserResponseModel>>> GetTeamMembers(string teamId)
        {
            List<User> teamMembers = await _teamService.GetAllTeamMembers(teamId);

            var models = _mapper.Map<List<UserResponseModel>>(teamMembers);

            return models;
        }

        [HttpDelete("RemoveUserFromTeam/{teamId}&{userId}")]
        public  async Task<IActionResult> RemoveUserFromTeam(string teamId, string userId)
        {
            User userToDelete = await _userService.GetUserWithIdAsync(userId);

            await _teamService.RemoveUserFromTeam(teamId, userToDelete);

            return Ok();
        }



    }
}
