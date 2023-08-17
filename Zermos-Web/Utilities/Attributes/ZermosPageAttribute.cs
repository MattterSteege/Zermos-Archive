using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Zermos_Web.Models.Requirements;

public class ZermosPageAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            base.OnActionExecuting(context);
        else
            context.Result = new RedirectResult("/?url=" + UrlEncoder.Default.Encode(context.HttpContext.Request.Path + context.HttpContext.Request.QueryString));
        
    }
}