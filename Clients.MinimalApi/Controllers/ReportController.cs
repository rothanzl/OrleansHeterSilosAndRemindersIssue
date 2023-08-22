using System.Web;
using Clients.MinimalApi.Bomber;
using Microsoft.AspNetCore.Mvc;

namespace Clients.MinimalApi.Controllers;


public class ReportController : ControllerBase
{
    private readonly Tester _tester;

    public ReportController(Tester tester)
    {
        _tester = tester;
    }


    [Produces("text/html")]
    [HttpGet("/tester/result/{path}")]
    public IActionResult GetResult([FromRoute] string path = "last")
    {
        if (path.Equals("last"))
        {
            path = Reports.GetLogDirectories().FirstOrDefault() ?? "";
        }
        
        
        var decodedPath = HttpUtility.UrlDecode(path);
        var result = Reports.GetHtmlReport(decodedPath);

        return Ok(result);
    }


    [HttpGet("/tester/results")]
    public IActionResult GetResults()
    {
        return Ok(Reports.GetLogDirectories());
    }

    [HttpGet("/tester/state")]
    public IActionResult GetState()
    {
        return Ok(_tester.State().ToString());
    }

    [HttpPost("/tester/start")]
    public IActionResult StartTester([FromBody] StartTestRequest req)
    {
        return Ok(_tester.Start(req).ToString());
    }

}