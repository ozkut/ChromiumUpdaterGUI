using System;
using System.Windows;
using Microsoft.Toolkit.Uwp.Notifications;

namespace ChromiumUpdater
{
    public partial class App : Application
    {
#pragma warning disable CS8618
        private static MainWindow mainWindow;
#pragma warning restore CS8618

        private App()
        {
            Updater.SettingUp = true;
            Updater.Client = new();
            ToastNotificationManagerCompat.OnActivated += toastArgs =>
            {
                //ToastArguments args = ToastArguments.Parse(toastArgs.Argument);
                //Windows.Foundation.Collections.ValueSet userInput = toastArgs.UserInput;
                Current.Dispatcher.Invoke(delegate
                {
                    switch (toastArgs.Argument[7..])
                    {
                        case "notificationClicked":
                            mainWindow.ShowWindowClicked(null!, null!);
                            break;
                        case "update":
                            Updater.CheckAndDownload(true);
                            break;
                        case "cancel":
                            Updater.Source.Cancel();
                            break;
                    }
                });
            };
            Updater.NotifyIcon = new() 
            { 
                Visible = true,
                Icon = System.Drawing.SystemIcons.Application,
                Text = Constants.Other.appTitle,
                ContextMenuStrip = new()
            };
            mainWindow = Updater.MainWindow = ChromiumUpdater.MainWindow.MainWindowInstance;
            Dispatcher.Invoke(Init);
            Updater.NotifyIcon.DoubleClick += mainWindow.ShowWindowClicked!;
        }

        /// <summary>
        /// Initial setup.
        /// </summary>
        private static void Init()
        {
            mainWindow.Hide();
            AddContextItems();
            Updater.CheckForRunningInstances(Constants.Other.appTitle);
            CheckStoredVariables();
            Updater.CheckAndDownload();
            Updater.SetRegularUpdateCheckInterval(mainWindow.cb_CheckUpdateRegularly.IsChecked!.Value);
            Updater.UpdateFileAttributes(mainWindow.cb_HideConfig.IsChecked!.Value);
            CheckErrorLog(System.IO.Path.Combine(Constants.StoredVariables.directory, Constants.Other.errorLogFileName));
            Updater.Secret(true);
            Updater.SettingUp = false;
            Updater.ShowDebugWarning();
        }

        /// <summary>
        /// Adds context menu items to the NotifyIcon.
        /// </summary>
        private static void AddContextItems()
        {
            (string, System.Drawing.Image, EventHandler)[] groupList =
            [
                ("Show main window", null, mainWindow.ShowWindowClicked!)!,
                ("Check for Chromium updates", null, mainWindow.CheckUpdateClicked!)!,
                ("Check for self update", null, mainWindow.CheckSelfUpdateClicked!)!,
                ("Exit", System.Drawing.SystemIcons.Error.ToBitmap(), mainWindow.ExitClicked!)
            ];
            for (int i = 0; i < groupList.Length; i++)
                _ = Updater.NotifyIcon.ContextMenuStrip!.Items.Add(groupList[i].Item1, groupList[i].Item2, groupList[i].Item3);
        }

        /// <summary>
        /// Checks for the existence of storedvariables and updates the UI if it does.
        /// </summary>
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

        /// <summary>
        /// Checks if an error log exists from a previous crash and asks the user if they want to see and delete it.
        /// </summary>
        /// <param name="errorLogPath"></param>
        private static void CheckErrorLog(string errorLogPath)
        {
            if (Updater.ErrorLogPath(errorLogPath) == string.Empty)
                return;
            MessageBoxResult openResult = MessageBox.Show("An error log from a previous crash has been found.\n" +
                                                          "Would you like to open it?",
                                                          Constants.Other.appTitle, MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (openResult == MessageBoxResult.Yes)
                System.Diagnostics.Process.Start("notepad", errorLogPath).WaitForExit();
            MessageBoxResult deleteResult = MessageBox.Show("Would you like to delete the error log?",
                                                            Constants.Other.appTitle, MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (deleteResult == MessageBoxResult.Yes)
                System.IO.File.Delete(errorLogPath);
        }
    }
}