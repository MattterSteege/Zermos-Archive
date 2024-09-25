namespace Zermos_Native_Windows;

static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // Check if the "--debug-version" flag is passed
        bool isDebugVersion = args.Contains("--debug-version");
        string? deepLink = args.FirstOrDefault(arg => arg.StartsWith("somtoday://"));

        // Pass the flag to the form
        Application.Run(new MainForm(isDebugVersion, deepLink));
    }
}