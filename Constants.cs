using System.IO;
namespace ChromiumUpdater
{
    internal static class Constants
    {
        protected internal struct Other
        {
            internal const string appTitle = "Chromium Updater";
            internal const string launcherTitle = "Chromium Updater Launcher";

            internal const string errorLogFileName = "ChromiumUpdater_Error.Log";

            internal const string currentVersion = "Current version: ";
            internal const string newestVersion = "Newest version: ";

            internal const string chr_InstallerFileName = "mini_installer.sync.exe";

            internal const int maxHttpClientTimeout = 5;
        }
        protected internal struct StoredVariables
        {
#if !DEBUG
            internal static readonly string directory = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), Other.appTitle);
            internal static readonly string configPath = Path.Combine(directory, "ChromiumUpdater_Config.json");
#else
            internal static readonly string directory = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), Other.appTitle) + "\\Debug";
            internal static readonly string configPath = Path.Combine(directory, "DEBUG_Config.json");
#endif
        }
        protected internal struct Paths
        {
            internal static readonly string installPath = Path.Combine(StoredVariables.directory, Other.appTitle + ".exe");
            internal static readonly string launcherInstallPath = Path.Combine(StoredVariables.directory, "Launcher.exe");
            internal static readonly string tempLauncherInstallPath = Path.Combine(StoredVariables.directory, "LauncherTemp.exe");
            
            internal static readonly string currentPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            internal static readonly string chr_InstallerFileLocation = Path.Combine(Path.GetTempPath(), Other.chr_InstallerFileName);
        }
        protected internal struct Links
        {
            internal const string downloadLink = "https://github.com/Hibbiki/chromium-win64/releases/latest/download/mini_installer.sync.exe";
            internal const string VersionLink = "https://omahaproxy.appspot.com/win";
        }
    }
}