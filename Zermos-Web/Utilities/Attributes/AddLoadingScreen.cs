using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Zermos_Web.Models.Requirements
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AddLoadingScreen : ActionFilterAttribute
    {
        public string LaadTekst { get; private set; }

        public AddLoadingScreen(string laadTekst)
        {
            LaadTekst = laadTekst;
        }
        
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                context.HttpContext.Items["loading"] = false;
                base.OnActionExecuting(context);
                return;
            }

            //make a string of the parameters passed to the method (if any)
            var parameters = "";
            foreach (var parameter in context.ActionArguments)
                parameters += parameter.Key + "=" + parameter.Value + "&";
            
            
            // Set ViewData values
            context.HttpContext.Items["loading"] = true;
            context.HttpContext.Items["laad_tekst"] = LaadTekst;
            context.HttpContext.Items["url"] = "/" + context.RouteData.Values["controller"] + "/" + context.RouteData.Values["action"] + "?" + parameters;

            // Return the _loading.cshtml view
            context.Result = new ViewResult
            {
                ViewName = "_loading"
            };
        }
    }
}