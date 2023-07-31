namespace Clients.MinimalApi.Bomber;

public class Reports
{
    
    public string[] GetLogDirectories() => Directory.GetDirectories("reports");

    public string GetHtmlReport(string path)
    {
        var files = Directory.GetFiles(path);

        var file = files.FirstOrDefault(f => f.ToLower().EndsWith(".html"));
        if (file is null)
            return string.Empty;

        return File.ReadAllText(file);
    }
    
}