using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Mvc.Filters;
using Zermos_Web.Utilities;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class CompressJavaScriptAttribute : ActionFilterAttribute
{
    public override void OnActionExecuted(ActionExecutedContext filterContext)
    {
        var response = filterContext.HttpContext.Response;
        var originalStream = response.Body;
        response.Body = new CompressJavaScriptAttributeResponseFilter(originalStream);
        base.OnActionExecuted(filterContext);
    }

    private class CompressJavaScriptAttributeResponseFilter : MemoryStream
    {
        private readonly Stream _originalStream;
        string html;

        public CompressJavaScriptAttributeResponseFilter(Stream originalStream)
        {
            _originalStream = originalStream;
            html = string.Empty;
        }
        
        public override void Write(byte[] buffer, int offset, int count)
        {
            html += Encoding.UTF8.GetString(buffer, offset, count);
            
            var scripts = Minifiers.GetScripts(html);
            if (scripts.Count == 0) return;
            foreach (var script in scripts)
            {
                var minified = Minifiers.MinifyJavaScript(script);
                html = html.Replace(script, minified);
            }
            
            buffer = Encoding.UTF8.GetBytes(html);
            _originalStream.WriteAsync(buffer, 0, buffer.Length);
        }
    }
}