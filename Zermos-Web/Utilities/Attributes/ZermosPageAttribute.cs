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
            
            
            //add <script>['.status','#sidebar','#content','.top-bar'].some(s=>!document.querySelector(s))&&location.reload()</script> to the end of the response (if it is a X-Requested-With request, not a POST or PUT request, and not a no-framework request)
            
            // if (context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest") {
            //     context.HttpContext.Response.OnStarting(async () =>
            //     {
            //         var writer = new StreamWriter(context.HttpContext.Response.Body);
            //         await writer.WriteAsync("<script>['.status','#sidebar','#content','.top-bar'].some(s=>!document.querySelector(s))&&location.reload()</script>"); //add a script to reload the page if the page is not fully loaded
            //         await writer.FlushAsync();
            //     });
            // }
        }
        else
        {
            context.Result = new RedirectResult("/?url=" + UrlEncoder.Default.Encode(context.HttpContext.Request.Path + context.HttpContext.Request.QueryString));
        }
    }
}