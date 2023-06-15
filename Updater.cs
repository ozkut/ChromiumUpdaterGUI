using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace ChromiumUpdater
{
    internal static class Updater
    {
        internal static HttpClient Client { get; set; }//new()
        internal static System.Threading.CancellationTokenSource Source { get; set; }
        private static System.Timers.Timer Timer { get; set; }
        internal static bool? SettingUp { get; set; }
        internal static System.Windows.Forms.NotifyIcon NotifyIcon { get; set; }
        internal static MainWindow MainWindow { get; set; }//= MainWindow.MainWindowInstance;

        internal static string ErrorlogExists(string errorLogPath)
        {
            return File.Exists(errorLogPath) ? errorLogPath : string.Empty;
        }

        internal static void CheckForRunningInstances(string programName)
        {
            Process[] processes = Process.GetProcessesByName(programName);
            if (processes.Length > 1)
            {
                MessageBoxResult result =
                MessageBox.Show($"A running instance of {programName} has been found.\n" +
                                "Would you like to close it?", programName, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    for (int i = 0; i < processes.Length; i++)
                        processes[i].Kill();
                }
            }
        }

        internal static bool StoredVariablesExists()
        {
            bool fileExists = File.Exists(Constants.StoredVariables.configPath);
            if (!fileExists) _ = Directory.CreateDirectory(Constants.StoredVariables.directory);
            return fileExists;
        }

        internal static void ShowDebugWarning()
        {
#if DEBUG
            Task.Delay(500);
            MessageBox.Show("Program is currently in debug mode!", Constants.Other.appTitle + " Debug Mode", MessageBoxButton.OK, MessageBoxImage.Warning);
#else
            SettingUp = null;
            return;
#endif
        }

        internal static async void CheckAndDownload(bool ignoreCheck = false)
        {
            if (await ShouldDownloadNewestVersion() || ignoreCheck)
                await DownloadNewestVersion();
        }

        internal static void CheckStartupStatus(bool startOnBoot)
        {
            //inside HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run
            using Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            switch (startOnBoot)
            {
                case true:
                    regKey.SetValue(Constants.Other.launcherTitle, Constants.Paths.launcherInstallPath);
                    break;
                case false:
                    regKey.DeleteValue(Constants.Other.launcherTitle, false);
                    break;
            }
            regKey.Close();
            regKey.Dispose();
        }

        internal static async void UpdateStoredVariables()
        {
            SerializeableVariables vars = new()
            {
                StartOnBoot = MainWindow.cb_StartOnBoot.IsChecked.Value,
                CheckUpdateOnClick = MainWindow.cb_ChechForUpdatesOnMaximise.IsChecked.Value,
                HideConfig = MainWindow.cb_HideConfig.IsChecked.Value,
                CheckForSelfUpdate = MainWindow.cb_CheckForSelfUpdate.IsChecked.Value,
                DownloadTimeout = (int)MainWindow.combobox_Timeout.SelectedItem,
                CheckUpdateRegularly = MainWindow.cb_CheckUpdateRegularly.IsChecked.Value,
                UpdateCheckInterval = (int)MainWindow.combobox_RegularUpdateCheckInterval.SelectedItem,
                ShowNotifWhenUpToDate = MainWindow.cb_ShowNotifWhenUpToDate.IsChecked.Value
            };
            await File.WriteAllTextAsync(Constants.StoredVariables.configPath, JsonSerializer.Serialize(vars, new JsonSerializerOptions { WriteIndented = true }));
            UpdateFileAttributes(MainWindow.cb_HideConfig.IsChecked.Value);
        }

        internal static void UpdateFileAttributes(bool hideFile)
        {
            if (!File.Exists(Constants.StoredVariables.configPath))
                return;
            FileAttributes attributes = File.GetAttributes(Constants.StoredVariables.configPath);
            File.SetAttributes(Constants.StoredVariables.configPath, hideFile ? attributes | FileAttributes.Hidden : attributes &= ~FileAttributes.Hidden);
        }

        internal static async void UpdateUIFromConfig()
        {
            UpdateFileAttributes(false);
            while (true)
            {
                try
                {
                    SerializeableVariables vars = JsonSerializer.Deserialize<SerializeableVariables>(await File.ReadAllTextAsync(Constants.StoredVariables.configPath));
                    MainWindow.cb_StartOnBoot.IsChecked = vars.StartOnBoot;
                    MainWindow.cb_ChechForUpdatesOnMaximise.IsChecked = vars.CheckUpdateOnClick;
                    MainWindow.cb_HideConfig.IsChecked = vars.HideConfig;
                    MainWindow.cb_CheckForSelfUpdate.IsChecked = vars.CheckForSelfUpdate;
                    MainWindow.combobox_Timeout.SelectedItem = vars.DownloadTimeout;
                    MainWindow.cb_CheckUpdateRegularly.IsChecked = vars.CheckUpdateRegularly;
                    MainWindow.combobox_RegularUpdateCheckInterval.SelectedItem = vars.UpdateCheckInterval;
                    MainWindow.cb_ShowNotifWhenUpToDate.IsChecked = vars.ShowNotifWhenUpToDate;
                    break;
                }
                catch { UpdateStoredVariables(); }
            }
            UpdateFileAttributes(MainWindow.cb_HideConfig.IsChecked.Value);
        }

        private static async void InvokeCheckUpdate(object sender, System.Timers.ElapsedEventArgs e) => await ShouldDownloadNewestVersion();

        internal static void CheckForUpdatesRegularly(bool enabled)
        {
            //I may have broken this
            if (!enabled)
            {
                MainWindow.cb_CheckUpdateRegularly.Content = "Check for updates regularly";
                Timer?.Stop();
                Timer?.Dispose();
                return;
            }

            MainWindow.cb_CheckUpdateRegularly.Content = "Check for updates regularly every                hour(s)";
            
            Timer?.Stop();
            Timer?.Dispose();

            Timer = new()
            {
                Interval = TimeSpan.FromHours((int)MainWindow.combobox_RegularUpdateCheckInterval.SelectedItem).TotalMilliseconds
            };
            Timer.Start();
            Timer.Elapsed += InvokeCheckUpdate;
        }

        internal static void ExitProgram()
        {
            NotifyIcon.Visible = false;
            NotifyIcon.Dispose();
            Client.CancelPendingRequests();
            Client.Dispose();
            Source?.Cancel(); 
            Source?.Dispose();
            Timer?.Dispose();
            Environment.Exit(Environment.ExitCode);
        }

        internal static async void Secret(bool isEnabled)
        {
            if (!isEnabled)
                return;
            if (DateTime.Today.ToString("M") == "28 March")
            {
                NotifyIcon.ShowBalloonTip(0, Constants.Other.appTitle + "'s creator", "Happy birthday to me!", System.Windows.Forms.ToolTipIcon.None);
                await Task.Delay(1000);
                _ = await Task.Run(() => Process.Start(new ProcessStartInfo("https://www.youtube.com/watch?v=dQw4w9WgXcQ") { UseShellExecute = true, CreateNoWindow = true, WindowStyle = ProcessWindowStyle.Maximized }));
                _ = MessageBox.Show("Happy birthday to me!", Constants.Other.appTitle, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private static async Task<bool> ShouldDownloadNewestVersion()
        {
            string newestVersion = string.Empty, currentVersion = string.Empty;
            bool hasInternet = false;

            try
            {
                newestVersion = await Client.GetStringAsync(Constants.Links.VersionLink);
                currentVersion = FileVersionInfo.GetVersionInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Chromium\Application\chrome.exe")).ProductVersion;
                hasInternet = true;
            }
            catch (HttpRequestException e)
            {
                DisplayErrorMessage("Can't get the latest version number of Chromium.\n" +
                                    "Please check your internet connection.", e);
                hasInternet = false;
            }
            catch (FileNotFoundException)
            {
                _ = MessageBox.Show($"A valid install of Chromium could not be found at '{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Chromium"}'\n" +
                                    "Please make sure that Chromium is installed at thet location.",
                                    Constants.Other.appTitle,
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);

                MessageBoxResult installResult =
                MessageBox.Show("Would you like to install Chromium?",
                                Constants.Other.appTitle,
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Question);

                if (installResult == MessageBoxResult.Yes)
                {
                    return true;//await DownloadNewestVersion();
                    //await CheckForUpdate();
                }
            }
            catch (Exception e) { DisplayErrorMessage("An unknown error has occured.\n" + e.Message, e); }

            MainWindow.l_CurrentVersion.Content = Constants.Other.currentVersion + currentVersion;
            MainWindow.l_NewestVersion.Content = Constants.Other.newestVersion + newestVersion;

            if (!hasInternet)
                return false;
            if (currentVersion.Equals(newestVersion))//haven't tested this at all
            {
                const string upToDateText = "Chromium is up-to-date!";
                if (!MainWindow.IsVisible && MainWindow.cb_ShowNotifWhenUpToDate.IsChecked.Value)
                    NotifyIcon.ShowBalloonTip(1, Constants.Other.appTitle, upToDateText, System.Windows.Forms.ToolTipIcon.None);
                else if (MainWindow.IsVisible)
                    _ = MessageBox.Show(upToDateText, Constants.Other.appTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                MainWindow.b_DownloadWhenUpToDate.IsEnabled = true;
                return false;
            }
            else
            {
                MainWindow.b_DownloadWhenUpToDate.IsEnabled = false;
                if (!MainWindow.IsVisible)
                    NotifyIcon.ShowBalloonTip(1, Constants.Other.appTitle, "An update is available!", System.Windows.Forms.ToolTipIcon.Info);
                else
                {
                    MessageBoxResult updateAvailableDialog =
                    MessageBox.Show($"Current Version: {currentVersion}\n" +
                                    $"Newest version available: {newestVersion}\n" +
                                    "Would you like to update?",
                                    Constants.Other.appTitle,
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Question);

                    if (updateAvailableDialog == MessageBoxResult.Yes)
                        return true;//await DownloadNewestVersion();
                }
                return false;
            }
        }

        private static async Task DownloadNewestVersion()
        {
            Source = new();
            string trimmedFileLocation = Constants.Paths.chr_InstallerFileLocation.Trim(Constants.Other.chr_InstallerFileName.ToCharArray());
            Progress<(float, float, DateTime)> progress = new(MainWindow.UpdateProgress);

            MainWindow.ChangeDownloadUIVisibility(true);

            try
            {
                using FileStream file = new(Constants.Paths.chr_InstallerFileLocation, FileMode.Create, FileAccess.Write, FileShare.None);
                await Client.DownloadAsync(Constants.Links.downloadLink, file, DateTime.Now, progress, Source.Token);
                file.Close();
            }
            catch (HttpRequestException e)
            {
                DisplayErrorMessage("A connection error has occured when trying to download the latest version of Chromium.\n" +
                                    "Please check your internet connection.", e);
            }
            catch (TaskCanceledException e)
            {
                MainWindow.ChangeDownloadUIVisibility(false);
                DisplayErrorMessage("The download operation has been cancelled.", e);
                DeleteTempChrInsaller();
                return;
            }
            catch (IOException e)
            {
                DisplayErrorMessage("An error occured when trying to write the installer file to " +
                                    $"'{trimmedFileLocation}'\n" +
                                    "Please make sure that the directory isn't read-only or that the file isn't opened by another program.", e);
            }
            catch (Exception e) { DisplayErrorMessage("An unknown error has occured.\n" + e.Message, e); }

            MainWindow.ChangeDownloadUIVisibility(false);

            //install update
            Process.Start(Constants.Paths.chr_InstallerFileLocation).WaitForExit();
            if (MainWindow.IsVisible)
                _ = MessageBox.Show("Install Complete!", Constants.Other.appTitle, MessageBoxButton.OK, MessageBoxImage.Information);

            DeleteTempChrInsaller();
            _ = await ShouldDownloadNewestVersion();
        }

        private static void DeleteTempChrInsaller()
        {
            MessageBoxResult deleteDialog =
            MessageBox.Show($"Would you like to delete the update file from '{Constants.Paths.chr_InstallerFileLocation}'?",
                            Constants.Other.appTitle,
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question);

            if (deleteDialog == MessageBoxResult.Yes)
                File.Delete(Constants.Paths.chr_InstallerFileLocation);
        }

        private static void DisplayErrorMessage(string errorMessage, Exception e)
        {
            if (e is HttpRequestException or TaskCanceledException)
            {
                _ = MessageBox.Show(errorMessage, Constants.Other.appTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string errorLogLocation = Path.Combine(Constants.StoredVariables.directory, Constants.Other.errorLogFileName);
            _ = MessageBox.Show("The program has crashed with the following exception:\n" + errorMessage +
                                $"\nAn error log has been created at: '{errorLogLocation}'",
                                Constants.Other.appTitle, MessageBoxButton.OK, MessageBoxImage.Error);

            if (!Directory.Exists(Constants.StoredVariables.directory))
                _ = Directory.CreateDirectory(Constants.StoredVariables.directory);

            File.WriteAllText(errorLogLocation, e.ToString());
            ExitProgram();
        }
    }
}