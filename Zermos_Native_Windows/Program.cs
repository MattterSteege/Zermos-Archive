namespace Zermos_Native_Windows;

static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        Application.ThreadException += Application_ThreadException;
        
        // Check if the "--debug-version" flag is passed
        bool isDebugVersion = args.Contains("--debug-version");
        string? deepLink = args.FirstOrDefault(arg => arg.StartsWith("somtoday://"));

        // Pass the flag to the form
        Application.Run(new MainForm(isDebugVersion, deepLink));
    }
    
    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Logger.Log($"Unhandled exception: {e.ExceptionObject}");
    }

    private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
    {
        Logger.Log($"Thread exception: {e.Exception}");
    }
}