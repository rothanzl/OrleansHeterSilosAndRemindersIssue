namespace Clients.MinimalApi.Bomber;

public static class Tester
{
    public static StartResult Start()
    {
        return new StartResult(State: "Started");
    }
}

public record StartResult(string State);