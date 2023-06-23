using System;
using System.Security.Claims;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Zermos_Web.Models.Requirements
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ZermeloRequirementAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = ((Users)context.HttpContext.RequestServices.GetService(typeof(Users)))?.GetUserAsync(context.HttpContext.User.FindFirstValue("email")).Result;
            
            if (string.IsNullOrEmpty(user.zermelo_access_token)) 
            {
                context.Result = new RedirectResult("/koppelingen/zermelo");
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}