using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkForceManagement.BLL.Services;
using WorkForceManagement.DAL.Entities;
using WorkForceManagement.DTO.RequestDTO;
using WorkForceManagement.DTO.ResponseDTO;

namespace WorkForceManagement.WEB.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateUserAsync(UserRequestDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = new User();
            _mapper.Map(model, user);

            await _userService.AddAsync(user, model.Password, model.IsAdmin);

            return Ok(model);
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUserAsync(Guid userId)
        {
            await _userService.DeleteAsync(userId);

            return Ok();
        }

        [HttpPatch("{userId}")]
        public async Task<IActionResult> UpdateUserAsync(Guid userId, UserRequestDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = new User();
            _mapper.Map(model, user);

            await _userService.EditAsync(userId, user, model.Password, model.IsAdmin);

            return Ok();
        }

        [HttpGet("All")]
        public async Task<ActionResult<List<UserResponseDTO>>> GetAllUsersAsync()
        {
            var users = await _userService.GetAllUsersAsync();

            var models = _mapper.Map<List<UserResponseDTO>>(users);

            return Ok(models);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDTO>> GetUserByIdAsync(Guid id)
        {
            var user = await _userService.GetUserWithIdAsync(id);
            var model = _mapper.Map<UserResponseDTO>(user);

            return Ok(model);
        }
    }
}
