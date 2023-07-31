using System.Web;
using Clients.MinimalApi.Bomber;
using Microsoft.AspNetCore.Mvc;

namespace Clients.MinimalApi.Controllers;


public class ReportController : ControllerBase
{

    [Produces("text/html")]
    [HttpGet("/tester/result/{path}")]
    public IActionResult GetResult([FromRoute] string path)
    {
        var decodedPath = HttpUtility.UrlDecode(path);
        var result = Reports.GetHtmlReport(decodedPath);

        return Ok(result);
    }


    [HttpGet("/tester/results")]
    public IActionResult GetResults()
    {

        return Ok(Reports.GetLogDirectories());
    }

}