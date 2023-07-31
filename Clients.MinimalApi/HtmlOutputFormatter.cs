using Microsoft.AspNetCore.Mvc.Formatters;

namespace Clients.MinimalApi;

public class HtmlOutputFormatter : StringOutputFormatter
{
    public HtmlOutputFormatter()
    {
        SupportedMediaTypes.Add("text/html");
    }
}