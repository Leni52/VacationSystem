using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkForceManagement.WEB.Policies
{
    public class TeamLeaderRequirement : IAuthorizationRequirement
    {
        public TeamLeaderRequirement()
        {

        }
    }
}
