using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class CustomCssMiddleware
{
    private readonly RequestDelegate _next;

    public CustomCssMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Get the requested file path
        string filePath = context.Request.Path.Value;

        // Check if the requested file has a .Css extension
        if (filePath.EndsWith(".min.css"))
        {
            
            //get the content of the css file (/css/... -> wwwroot/css/...)
            string fileContent = File.ReadAllText("wwwroot" + filePath.Replace(".min", ""));
            
            var length = fileContent.Length;
            
            if (string.IsNullOrWhiteSpace(fileContent)) return;

            // Remove comments
            fileContent = Regex.Replace(fileContent, @"/\*.*?\*/", string.Empty, RegexOptions.Singleline);

            // Remove whitespace around selectors, properties, and values
            fileContent = Regex.Replace(fileContent, @"\s*{\s*", "{");
            fileContent = Regex.Replace(fileContent, @"\s*}\s*", "}");
            fileContent = Regex.Replace(fileContent, @"\s*;\s*", ";");
            fileContent = Regex.Replace(fileContent, @"\s*:\s*", ":");
            fileContent = Regex.Replace(fileContent, @"\s*,\s*", ",");

            // Remove unnecessary semicolons
            fileContent = Regex.Replace(fileContent, @";\s*}", "}");

            // Remove newlines and multiple spaces
            fileContent = Regex.Replace(fileContent, @"\s+", " ");

#if DEBUG //show amount of bytes saved in debug mode
            fileContent += "\n/*" + (1 - fileContent.Length / (float) length) * 100 + "% space saved or " + (length - fileContent.Length) + " bytes saved*/";
#endif
                
            // Set the content type
            context.Response.ContentType = "text/css";

            // Write the custom CSS content to the response
            await context.Response.WriteAsync(fileContent.Trim());
        }
        else
        {
            // If the requested file doesn't have a .Css extension, proceed to the next middleware
            await _next(context);
        }
    }
}

// Extension method used to add the middleware to the HTTP request pipeline.
public static class CustomCssMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomCssMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CustomCssMiddleware>();
    }
}