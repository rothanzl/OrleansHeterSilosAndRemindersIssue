using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Silo.Grians;

namespace Silo.Controllers;


[Route("/")]
public class Controller : ControllerBase
{
    private readonly IGrainFactory _grainFactory;

    private static int Counter = 0;
    private static object CounterMutex = new();

    public Controller(IGrainFactory grainFactory)
    {
        _grainFactory = grainFactory;
    }


    [HttpGet("/")]
    public async Task<ActionResult<string>> GetIndex()
    {
        Stopwatch sw = Stopwatch.StartNew();

        int innerCounter;
        lock (CounterMutex)
        {
            Counter += 1;
            innerCounter = Counter;
        }

        sw.Stop();

        return Ok($"The success content of {innerCounter} within {sw.Elapsed}");
    }


    [HttpGet("hello/{name}")]
    public async Task<ActionResult<string>> SayHello([FromRoute] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "Name is empty!";


        var hello = await _grainFactory.GetGrain<IHelloGrain>(name).SayHello();

        return Ok(hello);
    }
    

}