using Microsoft.AspNetCore.Razor.TagHelpers;

namespace HonestAuto.TagHelpers
{
    [HtmlTargetElement("custom-href", Attributes = "page-url")]
    public class CustomHrefTagHelper : TagHelper
    {
        public string PageUrl { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "a"; // Change the tag from <custom-href> to <a>.
            output.Attributes.SetAttribute("href", PageUrl);
            output.TagMode = TagMode.StartTagAndEndTag; 
        }
    }
}
