using System;
using System.Security.Claims;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Zermos_Web.Utilities;

namespace Zermos_Web.Models.Requirements
{
    [AttributeUsage(AttributeTargets.Method)]
    public class InfowijsRequirement : ActionFilterAttribute
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
            
            if (string.IsNullOrEmpty(user.infowijs_access_token) || TokenUtils.GetTokenExpiration(user.infowijs_access_token) < DateTime.Now)
            {
                // User does not have the desired value, redirect to /a/login
                context.Result = new RedirectResult("/Koppelingen/Infowijs/Ongekoppeld");
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}