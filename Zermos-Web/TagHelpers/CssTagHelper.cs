using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;
using Zermos_Web.Controllers;

namespace Zermos_Web.TagHelpers
{
    [HtmlTargetElement("link", Attributes = "preload", TagStructure = TagStructure.WithoutEndTag)]
    public class CssTagHelper : TagHelper
    {
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            //set the .css extension to .min.css
            output.Attributes.SetAttribute("href", output.Attributes["href"].Value.ToString().Replace(".css", ".min.css"));
            output.Attributes.RemoveAll("preload");
        }
    }
}