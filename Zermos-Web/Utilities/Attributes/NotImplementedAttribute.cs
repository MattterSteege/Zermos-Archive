using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Zermos_Web.Models.Requirements
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class NotImplementedAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
#if release
            context.Result = new RedirectResult("/error/404");
#elif DEBUG
            base.OnActionExecuting(context);
#endif
        }
    }
}