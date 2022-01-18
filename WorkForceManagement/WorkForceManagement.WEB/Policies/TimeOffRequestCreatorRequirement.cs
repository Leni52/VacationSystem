using Microsoft.AspNetCore.Authorization;

namespace WorkForceManagement.WEB.Policies
{
    public class TimeOffRequestCreatorRequirement : IAuthorizationRequirement
    {
        public TimeOffRequestCreatorRequirement()
        {

        }
    }
}
