namespace Zermos_Native_Windows;

public class Logger
{
    private static string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Zermos", "log.txt");

    public static void Log(string message)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(logPath));
            File.AppendAllText(logPath, $"{DateTime.Now}: {message}\n");
        }
        catch (Exception ex)
        {
            // If we can't write to the log file, we're out of options
            System.Diagnostics.Debug.WriteLine($"Failed to write to log: {ex.Message}");
        }
    }
}