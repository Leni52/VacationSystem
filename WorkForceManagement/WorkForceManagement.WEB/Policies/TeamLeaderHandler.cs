using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WorkForceManagement.BLL.Services;
using WorkForceManagement.DAL.Entities;

namespace WorkForceManagement.WEB.Policies
{
    public class TeamLeaderHandler:AuthorizationHandler<TeamLeaderRequirement>
    {
        private IHttpContextAccessor _httpContextAccessor;
        private IAuthUserManager _authUserManager;
        private ITimeOffRequestService _timeOffRequestService;
        private IUserService _userService;
        public TeamLeaderHandler(IHttpContextAccessor httpContextAccessor, IAuthUserManager authUserManager,
            ITimeOffRequestService timeOffRequest, IUserService userService)
        {
            _httpContextAccessor = httpContextAccessor;
            _authUserManager = authUserManager;
            _timeOffRequestService = timeOffRequest;
            _userService = userService;
        }
       
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, TeamLeaderRequirement requirement)
        {
            Guid requestId = Guid.Parse(_httpContextAccessor.HttpContext.GetRouteValue("timeOffRequestId").ToString());
            string userId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            Guid userIdGuid = Guid.Parse(userId);

            User user = await _userService.GetUserById(userIdGuid);
            TimeOffRequest request = await _timeOffRequestService.GetTimeOffRequest(requestId);

            if (request == null || user == null)
            {
                context.Fail();
                await Task.CompletedTask;
                return;
            }
            if (request.Approvers.Contains(user))
            {
                context.Succeed(requirement);
                return;
            }            
            await Task.CompletedTask;
            return;
        }
    }
    }
