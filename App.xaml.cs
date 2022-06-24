using System.Diagnostics;
using System.IO;
using System.Windows;

namespace ChromiumUpdater
{
    public partial class App : Application
    {
        private static MainWindow mainWindow;

        private App() 
        {
            Updater.SettingUp = true;
            Updater.Client = new();
            mainWindow = Updater.MainWindow = ChromiumUpdater.MainWindow.MainWindowInstance;
            Dispatcher.Invoke(() => { Init(); });
        }

        private static void Init()
        {
            mainWindow.Hide();
            CheckStoredVariables();
            Updater.CheckAndDownload();
            Updater.CheckForUpdatesRegularly(mainWindow.cb_CheckUpdateRegularly.IsChecked.Value);
            Updater.UpdateFileAttributes(mainWindow.cb_HideConfig.IsChecked.Value);
            CheckErrorLog(Path.Combine(Constants.StoredVariables.directory, Constants.Other.errorLogFileName));
            Updater.CheckForRunningInstances(Constants.Other.appTitle);
            Updater.Secret(true);
            Updater.SettingUp = false;
            Updater.ShowDebugWarning();
        }

        private static void CheckStoredVariables()
        {
            if (Updater.StoredVariablesExists())
                Updater.UpdateUIFromConfig();
            else
            {
                Updater.UpdateStoredVariables();
                mainWindow.SetDefaultValues();
            }
        }

        private static void CheckErrorLog(string errorLogPath)
        {
            if (Updater.ErrorlogExists(errorLogPath) == string.Empty)
                return;
            MessageBoxResult openResult = MessageBox.Show("An error log from a previous crash has been found.\n" +
                                                          "Would you like to open it?",
                                                          Constants.Other.appTitle, MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (openResult == MessageBoxResult.Yes)
                Process.Start("notepad", errorLogPath).WaitForExit();
            MessageBoxResult deleteResult = MessageBox.Show("Would you like to delete the error log?",
                                                            Constants.Other.appTitle, MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (deleteResult == MessageBoxResult.Yes)
                File.Delete(errorLogPath);
        }
    }
}