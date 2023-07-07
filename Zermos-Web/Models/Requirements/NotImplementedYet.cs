using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Zermos_Web.Models.Requirements
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class NotImplementedYet : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.User.FindFirstValue("email") != "58373@ccg-leerling.nl")
                context.Result = new RedirectResult("/error/NotImplemented");
            else
                base.OnActionExecuting(context);
        }
    }
}