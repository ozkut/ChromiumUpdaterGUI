using System.IO;
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

            internal const string currentVersion = "Current version: ";
            internal const string newestVersion = "Newest version: ";

            internal const string chr_InstallerFileName = "mini_installer.sync.exe";

            internal const int maxUpdateCheckInterval = 5;
        }
        protected internal struct StoredVariables
        {
            #if !DEBUG
            internal readonly static string directory = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), Other.appTitle);
            internal readonly static string configPath = Path.Combine(directory, "ChromiumUpdater_Config.json");
            #else
            internal readonly static string directory = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), Other.appTitle) + "\\Debug";
            internal readonly static string configPath = Path.Combine(directory, "DEBUG_Config.json");
            #endif
        }
        protected internal struct Paths
        {
            internal readonly static string installPath = Path.Combine(StoredVariables.directory, Other.appTitle + ".exe");
            internal readonly static string launcherInstallPath = Path.Combine(StoredVariables.directory, "Launcher.exe");
            internal readonly static string tempLauncherInstallPath = Path.Combine(StoredVariables.directory, "LauncherTemp.exe");
            
            internal readonly static string currentPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            internal readonly static string chr_InstallerFileLocation = Path.Combine(Path.GetTempPath(), Other.chr_InstallerFileName);
        }
    }
}