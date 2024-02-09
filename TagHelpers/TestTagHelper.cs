using Microsoft.AspNetCore.Razor.TagHelpers;

namespace HonestAuto.TagHelpers
{
    public class TestTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context.TagName == "test")
            {
                output.TagName = "a";
                output.Attributes.SetAttribute("href", "https://discord.com/");
                output.Content.SetContent("Visit discord.com");
            }
        }
    }
}