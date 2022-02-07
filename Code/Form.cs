using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Text.Json;

//Chromium Updater made by Ozkut
//This is the GUI version of Chromium Updater

namespace ChromiumUpdaterGUI
{
    public partial class Form : System.Windows.Forms.Form
    {
        private static readonly NotifyIcon notifyIcon = new()
        {
            ContextMenuStrip = new ContextMenuStrip(),
            Icon = System.Drawing.SystemIcons.Application,
            Text = Constants.Other.appTitle,
            Visible = true
        };
        private static readonly HttpClient client = new();
        private static System.Threading.CancellationTokenSource source = new();
        
        public Form() => InitializeComponent();

        private void Form_Load(object sender, EventArgs e)
        {
            InitilizeStuff();
            notifyIcon.BalloonTipClicked += ShowWindowClicked;
            notifyIcon.DoubleClick += ShowWindowClicked;
        }

        //some methods
        private async void InitilizeStuff()
        {
            Hide();
            CheckStoredVariables();
            SetTimeout();
            AddContextItems();
            await CheckForUpdate();
            UpdateFileAttributes(cb_HideConfig.Checked);
            Secret(true);
            ShowDebugWarning();
        }

        private static async void ShowDebugWarning()
        {
            #if DEBUG
            await Task.Delay(500);
            MessageBox.Show("Program is currently in debug mode!", Constants.Other.appTitle + " Debug Mode", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            #endif
        }

        private void AddContextItems()
        {
            notifyIcon.ContextMenuStrip.Items.Add("Show main window", null, ShowWindowClicked);
            notifyIcon.ContextMenuStrip.Items.Add("Check for Chromium updates", null, CheckUpdateClicked);
            notifyIcon.ContextMenuStrip.Items.Add("Check for self update", null, CheckSelfUpdateClicked);
            notifyIcon.ContextMenuStrip.Items.Add("Exit", System.Drawing.SystemIcons.Error.ToBitmap(), ExitClicked);
        }

        private void SetTimeout()
        {
            numList.ValueChanged += TimeoutChanged;
            numList.Maximum = 5;
            numList.Minimum = 1;
            client.Timeout = TimeSpan.FromMinutes((double)numList.Value);
        }

        private async void ShowWindow()
        {
            Show();
            WindowState = FormWindowState.Normal;
            if (cb_CheckUpateOnClick.Checked)
                await CheckForUpdate();
        }

        private void UpdateProgressBar((float,float,DateTime) values)
        {
            //love you, tuples <3
            int percentage = (int)(values.Item2 / values.Item1 * 100);//%
            double downloaded = Math.Round(values.Item2 / 1000000);//MB
            double total = Math.Round(values.Item1 / 1000000);//MB

            TimeSpan elapsedTime = DateTime.Now - values.Item3;
            double downloadSpeed = downloaded / elapsedTime.TotalSeconds * 1000;//KB/s

            progressBar.Value = percentage;
            l_Progress.Text = $" {percentage}%";
            l_DownloadAmount.Text = $"{downloaded} MB / {total} MB";

            if (downloadSpeed < 1000)
                l_DownloadSpeed.Text = $"{downloadSpeed:0} KB/s";
            else
                l_DownloadSpeed.Text = $"{downloadSpeed / 1000:0.0} MB/s";
        }

        //some events
        private void ShowWindowClicked(object sender, EventArgs e) => ShowWindow();

        private async void CheckUpdateClicked(object sender, EventArgs e)
        {
            ShowWindow();
            await CheckForUpdate();
        }

        private void CheckSelfUpdateClicked(object sender, EventArgs e)
        {
            //this is also referanced(idk how to spell) in Form.Designer
            Process.Start(Constants.Paths.launcherInstallPath);
            ExitProgram();
        }

        private static void ExitClicked(object sender, EventArgs e) => ExitProgram();

        private void TimeoutChanged(object sender, EventArgs e) 
        {
            UpdateFileAttributes(false);
            UpdateStoredVariables();
            UpdateFileAttributes(cb_HideConfig.Checked);
        }

        //storedVaribles stuff
        private async void UpdateUIFromConfig()
        {
            UpdateFileAttributes(false);

            bool startOnBootState, checkUpdateOnClickState, hideConfigState, checkSelfUpdate;
            int timeOut;
            while (true)
            {
                try
                {
                    SerializeableVariables vars = JsonSerializer.Deserialize<SerializeableVariables>(await File.ReadAllTextAsync(Constants.StoredVariables.configPath));
                    startOnBootState = vars.StartOnBoot;
                    checkUpdateOnClickState = vars.CheckUpdateOnClick;
                    hideConfigState = vars.HideConfig;
                    checkSelfUpdate = vars.CheckForSelfUpdate;
                    timeOut = vars.DownloadTimeout;
                    break;
                }
                catch { UpdateStoredVariables(); }
            }

            cb_Startup.Checked = startOnBootState;
            cb_CheckUpateOnClick.Checked = checkUpdateOnClickState;
            cb_HideConfig.Checked = hideConfigState;
            cb_CheckSelfUpdate.Checked = checkSelfUpdate;
            numList.Value = timeOut;

            UpdateFileAttributes(cb_HideConfig.Checked);
        }

        private void CheckStoredVariables()
        {
            switch (File.Exists(Constants.StoredVariables.configPath))
            {
                case true:
                    UpdateUIFromConfig();
                    break;
                case false:
                    Directory.CreateDirectory(Constants.StoredVariables.directory);
                    UpdateStoredVariables();
                    break;
            }
        }

        private async void UpdateStoredVariables()
        {
            SerializeableVariables vars = new()
            {
                StartOnBoot = cb_Startup.Checked,
                CheckUpdateOnClick = cb_CheckUpateOnClick.Checked,
                HideConfig = cb_HideConfig.Checked,
                CheckForSelfUpdate = cb_CheckSelfUpdate.Checked,
                DownloadTimeout = (int)numList.Value
            };
            await File.WriteAllTextAsync(Constants.StoredVariables.configPath, JsonSerializer.Serialize(vars, new JsonSerializerOptions { WriteIndented = true }));
            UpdateFileAttributes(cb_HideConfig.Checked);
        }

        private static void UpdateFileAttributes(bool hideFile)
        {
            if (File.Exists(Constants.StoredVariables.configPath))
            {
                FileAttributes attributes = File.GetAttributes(Constants.StoredVariables.configPath);
                switch (hideFile)
                {
                    case true:
                        File.SetAttributes(Constants.StoredVariables.configPath, attributes | FileAttributes.Hidden);
                        break;
                    case false:
                        File.SetAttributes(Constants.StoredVariables.configPath, attributes &= ~FileAttributes.Hidden);
                        break;
                }
            }
        }

        //the original methods
        private static void CheckStartupStatus(bool startOnBoot)
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
        }

        private async Task CheckForUpdate()
        {
            string newestVersion = string.Empty;
            string currentVersion = string.Empty;
            bool hasInternet = false;

            try
            {
                newestVersion = await client.GetStringAsync("https://omahaproxy.appspot.com/win");
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
                MessageBox.Show($"A valid install of Chromium could not be found at '{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Chromium"}'\n" +
                                "Please make sure that Chromium is installed at thet location.\n",
                                Constants.Other.appTitle,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);

                DialogResult installResult =
                MessageBox.Show("Would you like to install Chromium?",
                                Constants.Other.appTitle,
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question);

                if (installResult == DialogResult.Yes)
                {
                    await DownloadNewestVersion();
                    await CheckForUpdate();//make this a Task.Run if any issues arrise
                }
            }
            catch (Exception e)
            {
                DisplayErrorMessage("An unknown error has occured.\n" +
                                    e.Message, e);
            }

            //don't know if this is necessary or if i just just hard-code the "newest version:" and "current version:"
            //into the strings rather than adding the version numbers on top and removing the extras every time
            //removing this later for a *cleaner* look and because it's probably unnecessary
            /*if (l_CurrentVersion.Text.Length > currentVersion.Length || l_NewestVersion.Text.Length > newestVersion.Length)
            {
                l_CurrentVersion.Text = l_CurrentVersion.Text.Trim(currentVersion.ToCharArray());
                l_NewestVersion.Text = l_NewestVersion.Text.Trim(newestVersion.ToCharArray());
            }*/

            l_CurrentVersion.Text = Constants.Other.currentVersion + currentVersion;
            l_NewestVersion.Text = Constants.Other.newestVersion + newestVersion;

            if (hasInternet)
            {
                switch (currentVersion == newestVersion)
                {
                    case true:
                        IsUpToDate(Visible);
                        break;

                    case false:
                        switch (Visible)
                        {
                            case false:
                                notifyIcon.ShowBalloonTip(Constants.Other.notificationTimeout, Constants.Other.appTitle, "An update is available!", ToolTipIcon.None);
                                break;

                            case true:
                                DialogResult updateAvailableDialog =
                                MessageBox.Show($"Current Version: {currentVersion}\n" +
                                                $"Newest version available: {newestVersion}\n" +
                                                "Would you like to update?",
                                                Constants.Other.appTitle,
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Question);

                                if (updateAvailableDialog == DialogResult.Yes)
                                    await DownloadNewestVersion();
                                break;
                        }
                        break;
                }
            }
        }
        
        private async Task DownloadNewestVersion()
        {
            notifyIcon.ShowBalloonTip(Constants.Other.notificationTimeout, Constants.Other.appTitle, "Downloading file", ToolTipIcon.Info);
            
            string trimmedFileLocation = Constants.Paths.chr_InstallerFileLocation.Trim(Constants.Other.chr_InstallerFileName.ToCharArray());
            Progress<(float,float,DateTime)> progress = new(UpdateProgressBar);
            
            ChangeDownloadUIVisibility(true);

            try
            {
                using FileStream file = new(Constants.Paths.chr_InstallerFileLocation, FileMode.Create, FileAccess.Write, FileShare.None);
                await client.DownloadAsync("https://github.com/Hibbiki/chromium-win64/releases/latest/download/mini_installer.sync.exe", file, DateTime.Now, progress, source.Token);
            }
            catch (HttpRequestException e)
            {
                DisplayErrorMessage("An HTTP error has occured when trying to download the latest version of Chromium.\n" +
                                    "Please check your internet connection.", e);
            }
            catch (TaskCanceledException e)
            {
                ChangeDownloadUIVisibility(false);
                DisplayErrorMessage("The download operation was cancelled.", e);
                DeleteTempChrInsaller();
                return;
            }
            catch (UnauthorizedAccessException e)
            {
                DisplayErrorMessage("The program does not have the proper permissions to write to " +
                                    $"'{trimmedFileLocation}'\n" +
                                    "Please check if the program has the correct permissions to write to the given directory.", e);
            }
            catch (DirectoryNotFoundException e)
            {
                DisplayErrorMessage($"The directory '{trimmedFileLocation}' could not be found.\n" +
                                    "Please make sure that the directory exists.", e);
            }
            catch (PathTooLongException e)
            {
                DisplayErrorMessage($"The path '{Constants.Paths.chr_InstallerFileLocation}' is too long.\n", e);
            }
            catch (IOException e)
            {
                DisplayErrorMessage("An error occured when trying to write the upadte file to " +
                                    $"'{trimmedFileLocation}'\n" +
                                    "Please make sure that the directory isn't read-only or the file isn't opened by another program.", e);
            }
            catch (Exception e)
            {
                DisplayErrorMessage("An unknown error has occured.\n" +
                                    e.Message, e);
            }

            ChangeDownloadUIVisibility(false);

            Process[] processes = Process.GetProcessesByName("chrome");
            if (processes.Length > 0)
            {
                DialogResult browserOpenDialog =
                    MessageBox.Show("An instance of Chromium is already running!\n" +
                                    "Would you like to close it?",
                                    Constants.Other.appTitle,
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Exclamation);

                if (browserOpenDialog == DialogResult.Yes)
                {
                    foreach (Process process in processes)
                        process.Kill();
                }
            }

            else
                await Task.Delay(500);
            
            InstallUpdate(Constants.Paths.chr_InstallerFileLocation, Visible);
            DeleteTempChrInsaller();
            await CheckForUpdate();
        }

        private static void IsUpToDate(bool isVisible)
        {
            const string upToDateText = "Chromium is up-to-date!";
            switch (isVisible)
            {
                case false:
                    notifyIcon.ShowBalloonTip(Constants.Other.notificationTimeout, Constants.Other.appTitle, upToDateText, ToolTipIcon.None);
                    break;
                case true:
                    MessageBox.Show(upToDateText, Constants.Other.appTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
            }      
        }

        private static void InstallUpdate(string fileLocation, bool isVisible)
        {
            Process.Start(fileLocation).WaitForExit();
            if (isVisible)
                MessageBox.Show("Install Complete!", Constants.Other.appTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static void DeleteTempChrInsaller()
        {
            DialogResult deleteDialog =
            MessageBox.Show($"Would you like to delete the update file from '{Constants.Paths.chr_InstallerFileLocation}'?",
                            Constants.Other.appTitle,
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

            if (deleteDialog == DialogResult.Yes)
                File.Delete(Constants.Paths.chr_InstallerFileLocation);
        }

        private void ChangeDownloadUIVisibility(bool show)
        {
            l_Progress.Visible = show;
            progressBar.Visible = show;
            l_DownloadAmount.Visible = show;
            b_CancelDownload.Visible = show;
            l_DownloadSpeed.Visible = show;
            if (!show)
            {
                progressBar.Value = 0;
                source = new();
            }
        }

        //checkboxes
        private void cb_Startup_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFileAttributes(false);
            UpdateStoredVariables();
            CheckStartupStatus(cb_Startup.Checked);
        }

        private void cb_CheckUpateOnClick_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFileAttributes(false);
            UpdateStoredVariables();
            UpdateFileAttributes(cb_HideConfig.Checked);
        }

        private void cb_HideConfig_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFileAttributes(false);
            UpdateStoredVariables();
            UpdateFileAttributes(cb_HideConfig.Checked);
        }

        private void cb_CheckSelfUpdate_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFileAttributes(false);
            UpdateStoredVariables();
            UpdateFileAttributes(cb_HideConfig.Checked);
        }

        //buttons
        private async void b_CheckUpdate_Click(object sender, EventArgs e) => await CheckForUpdate();

        private void b_Exit_Click(object sender, EventArgs e) => ExitProgram();

        private void b_DeleteConfig_Click(object sender, EventArgs e)
        {
            DialogResult deleteConfigResult =
            MessageBox.Show("Are you sure you want to delete the configuration file?\n",
                            Constants.Other.appTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (deleteConfigResult == DialogResult.Yes)
            {
                UpdateFileAttributes(false);
                if (File.Exists(Constants.StoredVariables.configPath))
                    File.Delete(Constants.StoredVariables.configPath);
            }
        }

        private void b_CancelDownload_Click(object sender, EventArgs e) => source.Cancel();

        //program end stuff
        private void Form_Closing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Hide();
                WindowState = FormWindowState.Minimized;
                e.Cancel = true;
            }
        }

        private void DisplayErrorMessage(string errorMessage, Exception e)
        {
            if (e is HttpRequestException || e is TaskCanceledException)
            {
                if (!Visible && e is not TaskCanceledException)
                    notifyIcon.ShowBalloonTip(Constants.Other.notificationTimeout, Constants.Other.appTitle, errorMessage, ToolTipIcon.Warning);
                else
                    MessageBox.Show(errorMessage, Constants.Other.appTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                string errorLogLocation = Path.Combine(Constants.StoredVariables.directory, Constants.Other.errorLogFileName);
                MessageBox.Show("The program has crashed with the following exception:\n" + errorMessage +
                                $"\nAn error log has been created at: '{errorLogLocation}'",
                                Constants.Other.appTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (!Directory.Exists(Constants.StoredVariables.directory))
                    Directory.CreateDirectory(Constants.StoredVariables.directory);

                client.Dispose();
                notifyIcon.Dispose();
                File.WriteAllText(errorLogLocation, e.ToString());
                Environment.Exit(1);
            }
        }

        private static void ExitProgram()
        {
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
            client.Dispose();
            source.Cancel();
            source.Dispose();
            Environment.Exit(0);
        }

        private async static void Secret(bool enabled)
        {
            if (enabled && DateTime.Today.ToString("M") == "28 March")
            {
                notifyIcon.ShowBalloonTip(Constants.Other.notificationTimeout, Constants.Other.appTitle, "Happy birthday to me!", ToolTipIcon.None);
                await Task.Delay(500);
                await Task.Run(() => Process.Start(new ProcessStartInfo("cmd", $"/c start " + "https://www.youtube.com/watch?v=dQw4w9WgXcQ".Replace("&", "^&")) { CreateNoWindow = true }));
                MessageBox.Show("Happy birthday to me!", Constants.Other.appTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }

    public static class CustomExtensions//copied from stackoverflow (https://stackoverflow.com/questions/20661652/progress-bar-with-httpclient)
    {
        public static async Task DownloadAsync(this HttpClient client, string requestUri, Stream destination, DateTime startTime, IProgress<(float,float,DateTime)> progress = null, System.Threading.CancellationToken cancellationToken = default)
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
            progress.Report((1,1,startTime));
        }
        public static async Task CopyToAsync(this Stream source, Stream destination, int bufferSize, IProgress<long> progress = null, System.Threading.CancellationToken cancellationToken = default)
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
                await destination.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken).ConfigureAwait(false);
                totalBytesRead += bytesRead;
                progress?.Report(totalBytesRead);
            }
        }
    }
}
//some very VERY bad code
//const string True = "True";
//const string False = "False";

//foreach (string line in File.ReadAllLines(Constants.storedVariables))
//{
//    switch (line)
//    {
//        case Constants.StartOnBoot + True:
//            cb_Startup.Checked = true;
//            break;

//        case Constants.StartOnBoot + False:
//            cb_Startup.Checked = false;
//            break;

//        case Constants.CheckUpdateOnClick + True:
//            cb_CheckUpateOnClick.Checked = true;
//            break;

//        case Constants.CheckUpdateOnClick + False:
//            cb_CheckUpateOnClick.Checked = false;
//            break;
//    }
//}

//some very VERY bad code
//if (File.Exists(Constants.storedVariables))
//{
//    UpdateFileAttributes(false);

//    int length = File.ReadAllLines(Constants.storedVariables).Length;
//    string[] final_StoredVariables = new string[length];

//    for (int i = 0; i < length; i++)
//    {
//        switch (i)
//        {
//            case 0:
//                final_StoredVariables[i] = Constants.StartOnBoot + cb_Startup.Checked;
//                break;

//            case 1:
//                final_StoredVariables[i] = Constants.CheckUpdateOnClick + cb_CheckUpateOnClick.Checked;
//                break;
//        }

//        for (int j = 0; j < final_StoredVariables.Length; j++)
//        {
//            if (j == 0)
//                await File.WriteAllTextAsync(Constants.storedVariables, final_StoredVariables[j] + '\n');
//            else
//                await File.AppendAllTextAsync(Constants.storedVariables, final_StoredVariables[j] + '\n');
//        }
//    }
//    UpdateFileAttributes(true);
//}

//else
//    CreateStoredVariables();

//not so bad but still, i found a better one!
//int currentVersionComparison = string.Compare(FileVersionInfo.GetVersionInfo(Constants.Paths.currentPath).ProductVersion, FileVersionInfo.GetVersionInfo(Constants.Paths.installPath).ProductVersion);
////if current version is smaller than installed version
//if  (currentVersionComparison > 0)
//{
//    DialogResult oldVersionFoundResult =
//    MessageBox.Show("The currently installed version of Chromium Updater is older than the version that has just been ran.\n" +
//                    "Would you like to replace it with the version currently running?",
//                    Constants.Other.appTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
//    if (oldVersionFoundResult == DialogResult.Yes)
//        File.Copy(Constants.Paths.currentPath, Constants.Paths.installPath, true);
//}
//else if (currentVersionComparison < 0)
//{

//}