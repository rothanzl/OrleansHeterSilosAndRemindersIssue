namespace Clients.MinimalApi.Bomber;

public static class Reports
{
    private const string DirName = "reports";
    
    
    public static string[] GetLogDirectories() => Directory
        .GetDirectories(DirName)
        .Select(s => s.Substring(DirName.Length + 1))
        .OrderByDescending(s => s)
        .ToArray();

    public static string GetHtmlReport(string path)
    {
        var files = Directory.GetFiles(Path.Join(DirName, path));

        var file = files.FirstOrDefault(f => f.ToLower().EndsWith(".html"));
        if (file is null)
            return string.Empty;

        return File.ReadAllText(file);
    }
    
}