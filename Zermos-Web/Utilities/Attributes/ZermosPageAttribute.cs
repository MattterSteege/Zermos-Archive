using System;
using System.IO;
using System.IO.Pipelines;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Zermos_Web.Controllers;

namespace Zermos_Web.Models.Requirements;

public class ZermosPageAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
            context.HttpContext.Request.Method == "POST" || context.HttpContext.Request.Method == "PUT" ||
            context.HttpContext.Request.Query.ContainsKey("no-framework"))
        {
            context.HttpContext.Items["dmjs"] = context.HttpContext.Request.Query.ContainsKey("dmjs") ? "//dmjs" : "";

            base.OnActionExecuting(context);
        }
        else
        {
            context.Result = new RedirectResult("/?url=" + UrlEncoder.Default.Encode(context.HttpContext.Request.Path + context.HttpContext.Request.QueryString));
        }
    }
}