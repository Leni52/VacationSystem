using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WorkForceManagement.BLL.Services.Interfaces;
using WorkForceManagement.DAL.Entities;

namespace WorkForceManagement.WEB.Policies
{
    public class TimeOffRequestCreatorHandler : AuthorizationHandler<TimeOffRequestCreatorRequirement>
    {
        private IHttpContextAccessor _httpContextAccessor;
        private IAuthUserManager _authUserManager;
        private ITimeOffRequestService _timeOffRequestService;
        private IUserService _userService;

        public TimeOffRequestCreatorHandler(IHttpContextAccessor httpContextAccessor, IAuthUserManager authUserManager,
            ITimeOffRequestService timeOffRequest, IUserService userService)
        {
            _httpContextAccessor = httpContextAccessor;
            _authUserManager = authUserManager;
            _timeOffRequestService = timeOffRequest;
            _userService = userService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, TimeOffRequestCreatorRequirement requirement)
        {
            Guid requestId = Guid.Parse(_httpContextAccessor.HttpContext.GetRouteValue("timeOffRequestId").ToString());
            string userId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            Guid userIdGuid = Guid.Parse(userId);

            User user = await _userService.GetUserById(userIdGuid);
            TimeOffRequest request = await _timeOffRequestService.GetTimeOffRequest(requestId);

            if (request == null || user == null)
            {
                context.Fail();
                await System.Threading.Tasks.Task.CompletedTask;
                return;
            }
            if (request.CreatorId == userId)
            {
                context.Succeed(requirement);
                return;
            }
            //unlimited admin access
            var getRole = _authUserManager.GetUserRoles(user);
            if (getRole.Result.Contains("Admin"))
            {
                context.Succeed(requirement);
                return;
            }
            await System.Threading.Tasks.Task.CompletedTask;
            return;
        }
    }
}