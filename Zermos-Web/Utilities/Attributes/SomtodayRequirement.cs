using System;
using System.Security.Claims;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Zermos_Web.Utilities;

namespace Zermos_Web.Models.Requirements
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SomtodayRequirementAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = ((Users)context.HttpContext.RequestServices.GetService(typeof(Users)))?.GetUserAsync(context.HttpContext.User.FindFirstValue("email")).Result;
            
            if (user == null) 
            {
                // User does not exist, redirect to /a/login
                context.Result = new RedirectResult("/Login");
                return;
            }
            
            if ((string.IsNullOrEmpty(user?.somtoday_access_token) && string.IsNullOrEmpty(user?.somtoday_refresh_token)) || TokenUtils.GetTokenExpiration(user.somtoday_refresh_token) < DateTime.Now)
            {
                // User does not have the desired value, redirect to /a/login
                context.Result = new RedirectResult("/Koppelingen/Somtoday/Ongekoppeld");
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}