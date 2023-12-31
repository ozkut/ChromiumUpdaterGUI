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
        internal static HttpClient Client { get; set; }
        internal static System.Threading.CancellationTokenSource Source { get; set; }
        private static System.Timers.Timer Timer { get; set; }
        internal static bool? SettingUp { get; set; }
        internal static System.Windows.Forms.NotifyIcon NotifyIcon { get; set; }
        internal static MainWindow MainWindow { get; set; }

        private static JsonSerializerOptions jsonOptions = new() { WriteIndented = true };

        /// <summary></summary>
        /// <param name="errorLogPath"></param>
        /// <returns>
        /// The path of the error log if it exists, if not, returns string.Empty.
        /// </returns>
        internal static string ErrorlogPath(string errorLogPath)
        {
            return File.Exists(errorLogPath) ? errorLogPath : string.Empty;
        }

        /// <summary>
        /// Checks for other running instances of the program and offers to kill the processes of those instances.
        /// </summary>
        /// <param name="programName"></param>
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

        /// <summary></summary>
        /// <returns>
        /// true if the storedvariables exists. Otherwise false.
        /// </returns>
        internal static bool StoredVariablesExists()
        {
            _ = Directory.CreateDirectory(Constants.StoredVariables.directory);
            return File.Exists(Constants.StoredVariables.configPath);
        }

        /// <summary>
        /// Displays an error message if the program is running in debug mode.
        /// </summary>
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

        /// <summary>
        /// Checks if there is a new version available and downloads it.
        /// </summary>
        /// <param name="ignoreCheck"></param>
        internal static async void CheckAndDownload(bool ignoreCheck = false)
        {
            if (await ShouldDownloadNewestVersion() || ignoreCheck)
                await DownloadNewestVersion();
        }

        /// <summary>
        /// Adds/removes a registry key inside HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run if the program should run at startup or not.
        /// </summary>
        /// <param name="startOnBoot"></param>
        internal static void CheckStartupStatus(bool startOnBoot)
        {
            //inside HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run
            using Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true)!;
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

        /// <summary>
        /// Updates the storedvariables file.
        /// </summary>
        internal static async void UpdateStoredVariables()
        {
            SerializableVariables vars = new()
            {
                StartOnBoot = MainWindow.cb_StartOnBoot.IsChecked!.Value,
                CheckUpdateOnClick = MainWindow.cb_ChechForUpdatesOnMaximise.IsChecked!.Value,
                HideConfig = MainWindow.cb_HideConfig.IsChecked!.Value,
                CheckForSelfUpdate = MainWindow.cb_CheckForSelfUpdate.IsChecked!.Value,
                DownloadTimeout = (int)MainWindow.combobox_Timeout.SelectedItem,
                CheckUpdateRegularly = MainWindow.cb_CheckUpdateRegularly.IsChecked!.Value,
                UpdateCheckInterval = (int)MainWindow.combobox_RegularUpdateCheckInterval.SelectedItem,
                ShowNotifWhenUpToDate = MainWindow.cb_ShowNotifWhenUpToDate.IsChecked!.Value
            };
            await File.WriteAllTextAsync(Constants.StoredVariables.configPath, JsonSerializer.Serialize(vars, jsonOptions));
            UpdateFileAttributes(MainWindow.cb_HideConfig.IsChecked.Value);
        }

        /// <summary>
        /// Hides/Unhides the storedvariables file.
        /// </summary>
        /// <param name="hideFile"></param>
        internal static void UpdateFileAttributes(bool hideFile)
        {
            if (!File.Exists(Constants.StoredVariables.configPath))
                return;
            FileAttributes attributes = File.GetAttributes(Constants.StoredVariables.configPath);
            File.SetAttributes(Constants.StoredVariables.configPath, hideFile ? attributes | FileAttributes.Hidden : attributes &= ~FileAttributes.Hidden);
        }

        /// <summary>
        /// Reads the storedvariables file and updates the variables of the current program instance.
        /// </summary>
        internal static async void UpdateUIFromConfig()
        {
            UpdateFileAttributes(false);
            while (true)
            {
                try
                {
                    SerializableVariables vars = JsonSerializer.Deserialize<SerializableVariables>(await File.ReadAllTextAsync(Constants.StoredVariables.configPath))!;
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

        /// <summary>
        /// Sets the regular update check interval.
        /// </summary>
        /// <param name="enabled"></param>
        internal static void SetRegularUpdateCheckInterval(bool enabled)
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

            Timer = new() { Interval = TimeSpan.FromHours((int)MainWindow.combobox_RegularUpdateCheckInterval.SelectedItem).TotalMilliseconds };
            Timer.Start();
            Timer.Elapsed += InvokeCheckUpdate!;
        }

        /// <summary>
        /// Gets the latest version from the chosen GitHub repo.
        /// </summary>
        /// <returns>
        /// The latest version or string.Empty if the program gets rate limited.
        /// </returns>
        private static async Task<string> GetLatestReleaseVersion()
        {
            try
            {
                System.Collections.Generic.IReadOnlyList<Octokit.Release> releases = await new Octokit.GitHubClient(new Octokit.ProductHeaderValue(Constants.Other.appTitle.Replace(' ', '-'))).Repository.Release.GetAll(Constants.Other.repoOwner, Constants.Other.repoName);
                return releases[0].TagName.Remove(0, 1);
            }
            catch (Octokit.RateLimitExceededException e) 
            { 
                DisplayErrorMessage("Too many requests sent!", e, false);
                return string.Empty;
            }
        }

        /// <summary>
        /// Checks if the newest version should be downloaded.
        /// </summary>
        /// <returns>
        /// true if the newest version can be successfully downloaded. false if any sort of error occurs or the installed version is already the latest one.
        /// </returns>
        private static async Task<bool> ShouldDownloadNewestVersion()
        {
            string newestVersion = string.Empty, currentVersion = string.Empty;
            bool hasInternet = false;

            try
            {
                newestVersion = await GetLatestReleaseVersion();
                currentVersion = FileVersionInfo.GetVersionInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Chromium\Application\chrome.exe")).ProductVersion!;
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
                                    "Please make sure that Chromium is installed at that location.",
                                    Constants.Other.appTitle,
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);

                MessageBoxResult installResult =
                MessageBox.Show("Would you like to install Chromium?",
                                Constants.Other.appTitle,
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Question);

                if (installResult == MessageBoxResult.Yes)
                    return true;
            }
            catch (Exception e) { DisplayErrorMessage("An unknown error has occurred.\n" + e.Message, e); }

            MainWindow.l_CurrentVersion.Content = Constants.Other.currentVersion + currentVersion;
            MainWindow.l_NewestVersion.Content = Constants.Other.newestVersion + newestVersion;

            if (!hasInternet || newestVersion == string.Empty)
                return false;

            if (currentVersion[..3] == newestVersion[..3])
            {
                string upToDateText = "Chromium is up-to-date!";
                if (currentVersion != newestVersion)
                    upToDateText = "Minor Chromium update available";
                if (!MainWindow.IsVisible && MainWindow.cb_ShowNotifWhenUpToDate.IsChecked!.Value)
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
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Downloads and installs the latest version while keeping track of the download progress.
        /// </summary>
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
                DisplayErrorMessage("A connection error has occurred when trying to download the latest version of Chromium.\n" +
                                    "Please check your internet connection.", e);
            }
            catch (TaskCanceledException e)
            {
                MainWindow.ChangeDownloadUIVisibility(false);
                DisplayErrorMessage("The download operation has been cancelled.", e);
                DeleteTempChrInstaller();
                return;
            }
            catch (IOException e)
            {
                DisplayErrorMessage("An error occurred when trying to write the installer file to " +
                                    $"'{trimmedFileLocation}'\n" +
                                    "Please make sure that the directory isn't read-only or that the file isn't opened by another program.", e);
            }
            catch (Exception e) { DisplayErrorMessage("An unknown error has occurred.\n" + e.Message, e); }

            MainWindow.ChangeDownloadUIVisibility(false);

            //install update
            Process.Start(Constants.Paths.chr_InstallerFileLocation).WaitForExit();
            if (MainWindow.IsVisible)
                _ = MessageBox.Show("Install Complete!", Constants.Other.appTitle, MessageBoxButton.OK, MessageBoxImage.Information);

            DeleteTempChrInstaller();
            _ = await ShouldDownloadNewestVersion();
        }

        /// <summary>
        /// Asks the user if they want to delete the installer file.
        /// </summary>
        private static void DeleteTempChrInstaller()
        {
            MessageBoxResult deleteDialog =
            MessageBox.Show($"Would you like to delete the update file from '{Constants.Paths.chr_InstallerFileLocation}'?",
                            Constants.Other.appTitle,
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question);

            if (deleteDialog == MessageBoxResult.Yes)
                File.Delete(Constants.Paths.chr_InstallerFileLocation);
        }

        /// <summary>
        /// Displays an error message and exits the program gracefully(?)
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="e"></param>
        /// <param name="isFatalError"></param>
        private static void DisplayErrorMessage(string errorMessage, Exception e, bool isFatalError = true)
        {
            if (e is HttpRequestException or TaskCanceledException)
            {
                _ = MessageBox.Show(errorMessage, Constants.Other.appTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string errorLogLocation = Path.Combine(Constants.StoredVariables.directory, Constants.Other.errorLogFileName);
            _ = MessageBox.Show("The program has encountered an error with the following message:\n" + errorMessage +
                                $"\nAn error log has been created at: '{errorLogLocation}'",
                                Constants.Other.appTitle, MessageBoxButton.OK, MessageBoxImage.Error);

            if (!Directory.Exists(Constants.StoredVariables.directory))
                _ = Directory.CreateDirectory(Constants.StoredVariables.directory);

            File.WriteAllText(errorLogLocation, e.ToString());
            if (isFatalError)
                ExitProgram();
        }

        /// <summary>
        /// Exits the program properly.
        /// </summary>
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

        /// <summary>
        /// heh
        /// </summary>
        /// <param name="isEnabled"></param>
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
    }
}