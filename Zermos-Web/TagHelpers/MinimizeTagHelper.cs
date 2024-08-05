using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;
using Zermos_Web.Controllers;

namespace Zermos_Web.TagHelpers
{
    [HtmlTargetElement("script", Attributes = "minimize")]
    public class MinimizeTagHelper : TagHelper
    {
        JsMinifier _jsMinifier = new();
        bool _hardToggleOff = false;

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            //setup
            output.TagName = "script";    // Replaces <script minimize> tag with <script> tag
            output.TagMode = TagMode.StartTagAndEndTag; // Ensures that the output includes both start and end tags
            output.Attributes.RemoveAll("minimize"); // Removes the minimize attribute from the output

            if (_hardToggleOff) return;

            //remove all the newlines and tabs
            var content = await output.GetChildContentAsync();
            var contentString = content.GetContent();
            
            if (contentString.Contains("//dmjs")) return; // Don't minimize dmjs scripts
            var length = contentString.Length;
            
            contentString = "\n" + _jsMinifier.Minify(contentString) + "\n";
            
#if DEBUG
            contentString += "//" + (1 - contentString.Length / (float) length) * 100 + "% space saved or " + (length - contentString.Length) + " bytes saved";
#endif
            //set the content
            output.Content.SetHtmlContent(contentString);
            
        }
    }
}