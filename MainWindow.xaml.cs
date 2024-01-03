using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace ChromiumUpdater
{
    public partial class MainWindow : Window
    {
        ///Singleton
        private static MainWindow tmp_MainWindowInstance = null!;
        private static readonly object mainWindowPadlock = new();
        internal static MainWindow MainWindowInstance
        {
            get
            {
                lock (mainWindowPadlock)
                {
                    tmp_MainWindowInstance ??= new();
                    return tmp_MainWindowInstance;
                }
            }
        }
        ///Singleton

        private TimeSpan previousSecond;

        private MainWindow()
        {
            InitializeComponent();
            combobox_RegularUpdateCheckInterval.Visibility = Visibility.Hidden;
            ChangeDownloadUIVisibility(false);
            SetTimeout(Constants.Other.maxHttpClientTimeout);
            SetRegularUpdateInterval();
        }

        /// <summary>
        /// Shows/Hides the download progress UI elements.
        /// </summary>
        /// <param name="show"></param>
        internal void ChangeDownloadUIVisibility(bool show)
        {
            taskbarInfo ??= new();
            taskbarInfo.ProgressState = show ? System.Windows.Shell.TaskbarItemProgressState.Normal : System.Windows.Shell.TaskbarItemProgressState.None;
            if (!show)
            {
                taskbarInfo.ProgressValue = 0;
                Progressbar.Value = 0;
                Updater.Source = new();
                CustomExtensions.canDownload = true;
            }
            l_Percentage.Visibility =  Progressbar.Visibility =  l_DownloadAmount.Visibility = b_CancelDownload.Visibility = l_DownloadSpeed.Visibility = l_DownloadState.Visibility = b_PauseDownload.Visibility = show ? Visibility.Visible : Visibility.Hidden;
        }

        /// <summary>
        /// Calculates the download progress and updates the UI elements.
        /// </summary>
        /// <param name="values"></param>
        internal void UpdateProgress((float, float, DateTime) values)
        {
            sbyte percentage = (sbyte)(values.Item2 / values.Item1 * 100);//%
            float downloaded = values.Item2 / 1000000;//MB
            float total = values.Item1 / 1000000;//MB

            string progress = $"{downloaded:0.0} MB / {total:0.0} MB";
            float progressflt = percentage / 100f;

            Progressbar.Value = percentage;
            taskbarInfo.ProgressValue = progressflt;
            l_Percentage.Content = $" {percentage}%";
            l_DownloadAmount.Content = progress;

            TimeSpan elapsedTime = DateTime.Now - values.Item3;
            if ((elapsedTime - previousSecond) >= TimeSpan.FromSeconds(1))//only update download speed if at least 1 second has passed
            {
                uint downloadSpeed = (uint)(downloaded / elapsedTime.TotalSeconds * 1000);//KB/s
                string downloadSpeedStr = downloadSpeed < 1000 ? $"{downloadSpeed:0} KB/s" : $"{downloadSpeed / 1000:0.0} MB/s";
                l_DownloadSpeed.Content = downloadSpeedStr;
                previousSecond = elapsedTime;
                Updater.UpdateToastProgress(taskbarInfo.ProgressValue.ToString(), $"Progress: {progress}", $"Speed: {downloadSpeedStr}");
            }
        }

        /// <summary>
        /// Resets all values to some pre-determined ones.
        /// </summary>
        internal void SetDefaultValues()
        {
            cb_StartOnBoot.IsChecked = false;
            cb_ChechForUpdatesOnMaximise.IsChecked = true;
            cb_HideConfig.IsChecked = true;
            cb_CheckForSelfUpdate.IsChecked = true;
            cb_CheckUpdateRegularly.IsChecked = true;
            combobox_RegularUpdateCheckInterval.SelectedItem = 60;
            cb_ShowNotifWhenUpToDate.IsChecked = true;
            combobox_Timeout.SelectedItem = Constants.Other.maxHttpClientTimeout;
            _ = Process.Start(Constants.Paths.installPath);
            Environment.Exit(Environment.ExitCode);
            //Updater.ExitProgram(0);//throws system can't find file error??
        }

        /// <summary>
        /// Initializes the timeout combobox.
        /// </summary>
        /// <param name="timeout"></param>
        private void SetTimeout(int timeout)
        {
            for (int i = 0; i < timeout; i++)
                combobox_Timeout.Items.Insert(i, i + 1);
            combobox_Timeout.SelectedItem = timeout;
        }

        /// <summary>
        /// Initializes the regular update interval combobox.
        /// </summary>
        private void SetRegularUpdateInterval()
        {
            int[] times = [1, 6, 12, 24];
            for (int i = 0; i < times.Length; i++)
                combobox_RegularUpdateCheckInterval.Items.Insert(i, times[i]);
            combobox_RegularUpdateCheckInterval.SelectedItem = times[^1];
        }

        /// <summary>
        /// Shows the main window and checks for updates if the user asked for it.
        /// </summary>
        private void ShowWindow()
        {
            Show();
            WindowState = WindowState.Normal;
            if (cb_ChechForUpdatesOnMaximise.IsChecked!.Value)
                Updater.CheckAndDownload();
        }

        /// <summary>
        /// Hides the main window instead of exiting the program when the user clicks the close window button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Hide();
            WindowState = WindowState.Minimized;
            e.Cancel = true;
        }

        //checkbox
        /// <summary>
        /// Updates the checkboxes after initial setup.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckboxChanged(object sender, RoutedEventArgs e)
        {
            if (Updater.SettingUp != true)
            {
                Updater.UpdateFileAttributes(false);
                Updater.UpdateStoredVariables();
                Updater.CheckStartupStatus(cb_StartOnBoot.IsChecked!.Value);
            }
        }

        /// <summary>
        /// Does the fancy combobox trick for the regular update combobox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cb_CheckForUpdatesRegularly_CheckChanged(object sender, RoutedEventArgs e)
        {
            if (cb_CheckUpdateRegularly.IsChecked!.Value)
            {
                combobox_RegularUpdateCheckInterval.Visibility = Visibility.Visible;
                //this *may* be useless (or not?)
                if (combobox_RegularUpdateCheckInterval.SelectedItem == null)
                {
                    SetRegularUpdateInterval();
                    Updater.UpdateUIFromConfig();
                }
            }
            else if (!cb_CheckUpdateRegularly.IsChecked.Value)
                combobox_RegularUpdateCheckInterval.Visibility = Visibility.Hidden;
            Updater.UpdateFileAttributes(false);
            Updater.UpdateStoredVariables();
            Updater.SetRegularUpdateCheckInterval(cb_CheckUpdateRegularly.IsChecked.Value);
            Updater.UpdateFileAttributes(cb_HideConfig.IsChecked!.Value);
        }

        //buttons
        internal void ShowWindowClicked(object sender, EventArgs e) => ShowWindow();

        internal void ExitClicked(object sender, EventArgs e) => Updater.ExitProgram();

        internal void CheckUpdateClicked(object sender, EventArgs e)
        {
            ShowWindow();
            Updater.CheckAndDownload();
        }

        internal void CheckSelfUpdateClicked(object sender, EventArgs e)
        {
            _ = Process.Start(Constants.Paths.launcherInstallPath);
            Updater.ExitProgram();
        }

        private void b_CheckChromiumUpdates_Clicked(object sender, RoutedEventArgs e) => Updater.CheckAndDownload();

        private void b_DeleteConfig_Clicked(object sender, RoutedEventArgs e)
        {
            MessageBoxResult deleteConfigResult =
            MessageBox.Show("Are you sure you want to delete the configuration file?\n",
                            Constants.Other.appTitle, MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (deleteConfigResult == MessageBoxResult.Yes)
            {
                Updater.UpdateFileAttributes(false);
                if (File.Exists(Constants.StoredVariables.configPath))
                    File.Delete(Constants.StoredVariables.configPath);
            }
        }

        private void b_CancelDownload_Clicked(object sender, RoutedEventArgs e) => Updater.Source.Cancel();

        private void b_OpenInstallFolder_Clicked(object sender, RoutedEventArgs e) => Process.Start("explorer", Constants.StoredVariables.directory);

        private void b_CheckSelfUpdate_Clicked(object sender, RoutedEventArgs e)
        {
            _ = Process.Start(Constants.Paths.launcherInstallPath);
            Updater.ExitProgram();
        }

        private void b_DefaultValues_Clicked(object sender, RoutedEventArgs e) => SetDefaultValues();

        private void b_DownloadWhenUpToDate_Clicked(object sender, RoutedEventArgs e) => Updater.CheckAndDownload(true);

        /// <summary>
        /// Handles the button side of the pausing/resuming of the download in progress.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void b_PauseDownload_Clicked(object sender, RoutedEventArgs e)
        {
            switch (CustomExtensions.canDownload)
            {
                case true:
                    b_PauseDownload.Content = "Resume Download";
                    taskbarInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Paused;
                    break;
                case false:
                    b_PauseDownload.Content = "Pause Download";
                    taskbarInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal;
                    break;
            }
            CustomExtensions.canDownload = !CustomExtensions.canDownload;
            l_DownloadState.Content = "Download Paused: " + !CustomExtensions.canDownload;
        }

        private void b_Exit_Clicked(object sender, RoutedEventArgs e) => ExitClicked(sender, e);

        //other
        private void combobox_RegularUpdateCheckInterval_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (Updater.SettingUp != true)
            {
                Updater.UpdateFileAttributes(false);
                Updater.UpdateStoredVariables();
                Updater.UpdateFileAttributes(cb_HideConfig.IsChecked!.Value);
                Updater.SetRegularUpdateCheckInterval(cb_CheckUpdateRegularly.IsChecked!.Value);
            }
        }

        private void combobox_Timeout_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (Updater.SettingUp != true)
            {
                Updater.UpdateFileAttributes(false);
                Updater.UpdateStoredVariables();
                Updater.UpdateFileAttributes(cb_HideConfig.IsChecked!.Value);
            }
        }
    }

    /// <summary>
    /// Some custom extension methods I've made.
    /// </summary>
    internal static class CustomExtensions
    {
        internal static bool canDownload = true;
        /// <summary>
        /// Implements the DownloadAsync method of HttpClient with support for my custom Progress property.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="requestUri"></param>
        /// <param name="destination"></param>
        /// <param name="startTime"></param>
        /// <param name="progress"></param>
        /// <param name="cancellationToken"></param>
        internal static async Task DownloadAsync(this HttpClient client, string requestUri, Stream destination, DateTime startTime, IProgress<(float, float, DateTime)> progress = null!, System.Threading.CancellationToken cancellationToken = default)
        {
            using HttpResponseMessage response = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            long? length = response.Content.Headers.ContentLength;
            using Stream download = await response.Content.ReadAsStreamAsync(cancellationToken);
            if (progress == null || !length.HasValue)
            {
                await download.CopyToAsync(destination, cancellationToken);
                return;
            }
            IProgress<long> finalProgress = new Progress<long>(totalBytes => progress.Report((length.Value, totalBytes, startTime)));
            await download.CopyToAsync(destination, 81920, finalProgress, cancellationToken);
            progress.Report((1, 1, startTime));
        }

        /// <summary>
        /// Implements the CopyToAsync method of Stream with support for pausing the download and a progress property.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="bufferSize"></param>
        /// <param name="progress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static async Task CopyToAsync(this Stream source, Stream destination, int bufferSize, IProgress<long> progress = null!, System.Threading.CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(destination);
            ArgumentOutOfRangeException.ThrowIfNegative(bufferSize);
            if (!source.CanRead)
                throw new ArgumentException("Has to be readable", nameof(source));
            if (!destination.CanWrite)
                throw new ArgumentException("Has to be writable", nameof(destination));

            byte[] buffer = new byte[bufferSize];
            int bytesRead, totalBytesRead = 0;
            while ((bytesRead = await source.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken).ConfigureAwait(false)) != 0)
            {
                if (canDownload)
                {
                    await destination.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken).ConfigureAwait(false);
                    totalBytesRead += bytesRead;
                    progress?.Report(totalBytesRead);
                }
            }
        }
    }
}