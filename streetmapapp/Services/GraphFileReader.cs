namespace StreetMapApp.Services;

internal static class GraphFileReader
{
    public static string Read(string dataPath)
    {
        if (!File.Exists(dataPath))
        {
            throw new FileNotFoundException($"Data file not found at: {dataPath}");
        }

        return File.ReadAllText(dataPath);
    }
}
