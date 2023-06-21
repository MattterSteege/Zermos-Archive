using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Zermos_Web.Utilities;

namespace Zermos_Web.Models.Requirements
{
    public class SomtodayRequirement : IAuthorizationRequirement
    {
        public readonly Users _users;

        public SomtodayRequirement(Users users)
        {
            _users = users;
        }
    }

    public class SomtodayAuthorizationHandler : AuthorizationHandler<SomtodayRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SomtodayRequirement requirement)
        {
            var user = requirement._users.GetUserAsync(context.User.FindFirstValue("email")).Result;

            if (TokenUtils.CheckToken(user.somtoday_access_token))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
            
            return Task.CompletedTask;
        }

    }

}