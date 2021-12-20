namespace ChromiumUpdaterGUI
{
    public static class Constants
    {
        public const string appTitle = "Chromium Updater";
        public readonly static string storedVariablesDirectory = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), appTitle);
        public readonly static string storedVariables = System.IO.Path.Combine(storedVariablesDirectory, "ChromiumUpdater_Config.json");
        public const int notificationTimeout = 0;//this is now depricated apparently
    }
}