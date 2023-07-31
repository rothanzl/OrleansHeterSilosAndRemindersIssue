using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Silo.Controllers;


[Route("/")]
public class Controller : ControllerBase
{

    private static int Counter = 0;
    private static object CounterMutex = new();
    

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

        for (int i = 0; i < innerCounter; i++)
        {
            
        }
        sw.Stop();
        
        
        return Ok($"The success content of {innerCounter} within {sw.Elapsed}");
    }

}