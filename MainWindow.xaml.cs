﻿using System;
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
        private static MainWindow tmp_MainWindowInstance = null;
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

        internal void ChangeDownloadUIVisibility(bool show)
        {
            taskbarInfo ??= new();
            Visibility visibility = show ? Visibility.Visible : Visibility.Hidden;
            taskbarInfo.ProgressState = show ? System.Windows.Shell.TaskbarItemProgressState.Normal : System.Windows.Shell.TaskbarItemProgressState.None;
            if (!show)
            {
                taskbarInfo.ProgressValue = 0;
                Progressbar.Value = 0;
                Updater.Source = new();
                CustomExtensions.canDownload = true;
            }
            l_Percentage.Visibility = visibility;
            Progressbar.Visibility = visibility;
            l_DownloadAmount.Visibility = visibility;
            b_CancelDownload.Visibility = visibility;
            l_DownloadSpeed.Visibility = visibility;
            l_DownloadState.Visibility = visibility;
            b_PauseDownload.Visibility = visibility;
        }

        internal void UpdateProgress((float, float, DateTime) values)
        {
            sbyte percentage = (sbyte)(values.Item2 / values.Item1 * 100);//%
            float downloaded = values.Item2 / 1000000;//MB
            float total = values.Item1 / 1000000;//MB

            Progressbar.Value = percentage;
            taskbarInfo.ProgressValue = percentage / 100f;
            l_Percentage.Content = $" {percentage}%";
            l_DownloadAmount.Content = $"{downloaded:0.0} MB / {total:0.0} MB";

            TimeSpan elapsedTime = DateTime.Now - values.Item3;
            if ((elapsedTime - previousSecond) >= TimeSpan.FromSeconds(1))//only update download speed if at least 1 second has passed
            {
                uint downloadSpeed = (uint)(downloaded / elapsedTime.TotalSeconds * 1000);//KB/s
                l_DownloadSpeed.Content = downloadSpeed < 1000 ? $"{downloadSpeed:0} KB/s" : $"{downloadSpeed / 1000:0.0} MB/s";
                previousSecond = elapsedTime;
            }
        }

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

        private void SetTimeout(int timeout)
        {
            for (int i = 0; i < timeout; i++)
                combobox_Timeout.Items.Insert(i, i + 1);
            combobox_Timeout.SelectedItem = timeout;
        }

        private void SetRegularUpdateInterval()
        {
            int[] times = { 1, 6, 12, 24 };
            for (int i = 0; i < times.Length; i++)
                combobox_RegularUpdateCheckInterval.Items.Insert(i, times[i]);
            combobox_RegularUpdateCheckInterval.SelectedItem = times[^1];
        }

        private void ShowWindow()
        {
            Show();
            WindowState = WindowState.Normal;
            if (cb_ChechForUpdatesOnMaximise.IsChecked.Value)
                Updater.CheckAndDownload();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Hide();
            WindowState = WindowState.Minimized;
            e.Cancel = true;
        }

        //checkbox
        private void CheckboxChanged(object sender, RoutedEventArgs e)
        {
            if (Updater.SettingUp != true)
            {
                Updater.UpdateFileAttributes(false);
                Updater.UpdateStoredVariables();
                Updater.CheckStartupStatus(cb_StartOnBoot.IsChecked.Value);
            }
        }

        private void cb_CheckForUpdatesRegularly_CheckChanged(object sender, RoutedEventArgs e)
        {
            if (cb_CheckUpdateRegularly.IsChecked.Value)
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
            Updater.CheckForUpdatesRegularly(cb_CheckUpdateRegularly.IsChecked.Value);
            Updater.UpdateFileAttributes(cb_HideConfig.IsChecked.Value);
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
        private void combobox_RegularUpdateCheckInterval_SellectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (Updater.SettingUp != true)
            {
                Updater.UpdateFileAttributes(false);
                Updater.UpdateStoredVariables();
                Updater.UpdateFileAttributes(cb_HideConfig.IsChecked.Value);
                Updater.CheckForUpdatesRegularly(cb_CheckUpdateRegularly.IsChecked.Value);
            }
        }

        private void combobox_Timeout_SellectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (Updater.SettingUp != true)
            {
                Updater.UpdateFileAttributes(false);
                Updater.UpdateStoredVariables();
                Updater.UpdateFileAttributes(cb_HideConfig.IsChecked.Value);
            }
        }
    }

    internal static class CustomExtensions
    {
        internal static bool canDownload = true;
        internal static async Task DownloadAsync(this HttpClient client, string requestUri, Stream destination, DateTime startTime, IProgress<(float, float, DateTime)> progress = null, System.Threading.CancellationToken cancellationToken = default)
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

        internal static async Task CopyToAsync(this Stream source, Stream destination, int bufferSize, IProgress<long> progress = null, System.Threading.CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!source.CanRead)
                throw new ArgumentException("Has to be readable", nameof(source));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (!destination.CanWrite)
                throw new ArgumentException("Has to be writable", nameof(destination));
            if (bufferSize < 0)
                throw new ArgumentOutOfRangeException(nameof(bufferSize));

            byte[] buffer = new byte[bufferSize];
            int totalBytesRead = 0;
            int bytesRead;
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