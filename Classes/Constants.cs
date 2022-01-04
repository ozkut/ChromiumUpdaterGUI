namespace ChromiumUpdaterGUI
{
    internal static class Constants
    {
        protected internal struct Other
        {
            internal const string appTitle = "Chromium Updater";
            internal const string launcherTitle = "Chromium Updater Launcher";
            internal const string errorLogFileName = "ChromiumUpdater_Error.Log";
            internal const int notificationTimeout = 3000;//this is now depricated apparently
        }
        protected internal struct StoredVariables
        {
            #if !DEBUG
            internal readonly static string directory = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), Other.appTitle);
            internal readonly static string filePath = System.IO.Path.Combine(directory, "ChromiumUpdater_Config.json");
            #else
            internal readonly static string directory = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), Other.appTitle) + "\\Debug";
            internal readonly static string filePath = System.IO.Path.Combine(directory, "DEBUG_Config.json");
            #endif
        }
        protected internal struct Paths
        {
            internal readonly static string installPath = System.IO.Path.Combine(StoredVariables.directory, Other.appTitle + ".exe");
            internal readonly static string launcherInstallPath = System.IO.Path.Combine(StoredVariables.directory, Other.appTitle + " Launcher.exe");
            internal readonly static string currentPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
        }
    }
}