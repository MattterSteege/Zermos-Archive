using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

public class CustomJsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly JsMinifier _jsMinifier = new JsMinifier();

    public CustomJsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Get the requested file path
        string filePath = context.Request.Path.Value;

        // Check if the requested file has a .js extension
        if (filePath.EndsWith(".min.js"))
        {
            // Read the content of the requested JavaScript file
            string fileContent = File.ReadAllText("wwwroot" + filePath.Replace(".min", ""));
            
            var length = fileContent.Length;
            
            fileContent = _jsMinifier.Minify(fileContent);
            
#if DEBUG
            fileContent += "\n//" + (1 - fileContent.Length / (float) length) * 100 + "% space saved\n";
#endif

            // Set the content type
            context.Response.ContentType = "text/javascript";

            // Write the custom JavaScript content to the response
            await context.Response.WriteAsync(fileContent);
        }
        else
        {
            // If the requested file doesn't have a .js extension, proceed to the next middleware
            await _next(context);
        }
    }
}

// Extension method used to add the middleware to the HTTP request pipeline.
public static class CustomJsMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomJsMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CustomJsMiddleware>();
    }
}