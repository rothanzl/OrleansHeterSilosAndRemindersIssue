using Microsoft.AspNetCore.Mvc;

namespace Dashboard.Controllers;

[Route("/")]
public class Controller : ControllerBase
{
    private readonly IGrainFactory _grainFactory;
    private readonly ILogger<Controller> _logger;

    public Controller(IGrainFactory grainFactory, ILogger<Controller> logger)
    {
        _grainFactory = grainFactory;
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<string> Root()
    {
        return Ok("Dashboard");
    }

}