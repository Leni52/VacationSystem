using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkForceManagement.BLL.Services;
using WorkForceManagement.DAL.Entities;
using WorkForceManagement.DTO.RequestModels;
using WorkForceManagement.DTO.ResponseModels;

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

        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUserAsync(UserRequestModel model)
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

        [HttpDelete("DeleteUser/{userId}")]
        public async Task<IActionResult> DeleteUserAsync(string userId)
        {
            await _userService.DeleteAsync(userId);

            return Ok();
        }

        [HttpPatch("UpdateUser/{userId}")]
        public async Task<IActionResult> UpdateUserAsync(string userId, UserRequestModel model)
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

        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<List<UserResponseModel>>> GetAllUsersAsync()
        {
            var users = await _userService.GetAllUsersAsync();

            var models = _mapper.Map<List<UserResponseModel>>(users);

            return Ok(models);
        }

        [HttpGet("GetUserById/{id}")]
        public async Task<ActionResult<UserResponseModel>> GetUserByIdAsync(string id)
        {
            var user = await _userService.GetUserWithIdAsync(id);
            var model = _mapper.Map<UserResponseModel>(user);

            return Ok(model);
        }
    }
}
