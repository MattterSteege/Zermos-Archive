using System;
using System.Security.Claims;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Zermos_Web.Controllers;

namespace Zermos_Web.Models.Requirements
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SomtodayRequirementAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = ((Users)context.HttpContext.RequestServices.GetService(typeof(Users)))?.GetUserAsync(context.HttpContext.User.FindFirstValue("email")).Result;
            
            if (string.IsNullOrEmpty(user?.somtoday_access_token) && string.IsNullOrEmpty(user?.somtoday_refresh_token))
            {
                // User does not have the desired value, redirect to /a/login
                context.Result = new RedirectResult("/koppelingen/somtoday");
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}