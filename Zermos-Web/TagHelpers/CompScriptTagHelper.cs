using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace Zermos_Web.TagHelpers 
{
    [HtmlTargetElement("comp-script")]
    public class CompScriptTagHelper : TagHelper
    {
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            // You can add your own logic here to handle the content of the comp-script tag
            // For example, you can manipulate the content or execute scripts
            
            // Get the content of the <comp-script> tag
            var content = await output.GetChildContentAsync();

            // Manipulate the content if needed
            var modifiedContent = content.GetContent().ToUpper(); // Example: Convert to uppercase

            // Set the modified content as the output content
            output.Content.SetHtmlContent(modifiedContent);
        }
    }
}