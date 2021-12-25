namespace ChromiumUpdaterGUI
{
    public static class Constants
    {
        public struct Other
        {
            public const string appTitle = "Chromium Updater";
            public const string errorLogFileName = "ChromiumUpdater_Error.Log";
            public const int notificationTimeout = 0;//this is now depricated apparently
        }
        public struct StoredVariables
        {
            #if DEBUG
            public readonly static string directory = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), Other.appTitle) + "\\Debug";
            #else
            public readonly static string directory = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), Other.appTitle);
            #endif
            public readonly static string filePath = System.IO.Path.Combine(directory, "ChromiumUpdater_Config.json");
        }
        public struct Paths
        {
            public readonly static string installPath = System.IO.Path.Combine(StoredVariables.directory, Other.appTitle + ".exe");
            public readonly static string currentPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
        }
    }
}