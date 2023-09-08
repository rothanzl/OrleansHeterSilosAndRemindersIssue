using Microsoft.AspNetCore.Mvc.Formatters;

namespace Tester;

public class HtmlOutputFormatter : StringOutputFormatter
{
    public HtmlOutputFormatter()
    {
        SupportedMediaTypes.Add("text/html");
    }
}