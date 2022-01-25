using Microsoft.AspNetCore.Authorization;

namespace WorkForceManagement.WEB.Policies
{
    public class TeamLeaderRequirement : IAuthorizationRequirement
    {
        public TeamLeaderRequirement()
        {
        }
    }
}