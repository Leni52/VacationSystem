using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateUser(UserRequestDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            User currentUser = await _userService.GetCurrentUser(User);

            var user = new User();
            _mapper.Map(model, user);
            user.CreatorId = currentUser.Id;

            await _userService.Add(user, model.Password, model.IsAdmin);

            return Ok(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            await _userService.Delete(userId);

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{userId}")]
        public async Task<IActionResult> UpdateUser(Guid userId, UserRequestDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            User currentUser = await _userService.GetCurrentUser(User);

            User userToUpdate = await _userService.GetUserById(userId);

            if (userToUpdate == null)
                throw new KeyNotFoundException($"User with Id:{userId} was not found");

            _mapper.Map(model, userToUpdate);
            userToUpdate.UpdaterId = currentUser.Id;

            await _userService.Update(userToUpdate, model.Password, model.IsAdmin);

            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<List<UserResponseDTO>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();

            var results = _mapper.Map<List<UserResponseDTO>>(users);

            return Ok(results);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDTO>> GetUserById(Guid id)
        {
            var user = await _userService.GetUserById(id);
            var result = _mapper.Map<UserResponseDTO>(user);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("MakeUserAdmin/{userId}")]
        public async Task<IActionResult> MakeUserAdmin(Guid userId)
        {
            var user = await _userService.GetUserById(userId);

            await _userService.MakeUserAdmin(user);

            return Ok(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("RemoveUserFromAdmin/{userId}")]
        public async Task<IActionResult> RemoveUserFromAdmin(Guid userId)
        {
            var user = await _userService.GetUserById(userId);

            await _userService.RemoveUserFromAdmin(user);

            return Ok(user);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {

            await _userService.ConfirmEmailAdress(userId, token);

            return Ok();
        }
    }
}
