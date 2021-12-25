using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Text.Json;
using Octokit;

//Chromium Updater made by Ozkut
//This is the GUI version of (the now discontinued) Chromium Updater

namespace ChromiumUpdaterGUI
{
    public partial class Form : System.Windows.Forms.Form
    {
        static readonly NotifyIcon notifyIcon = new()
        {
            ContextMenuStrip = new ContextMenuStrip(),
            Icon = System.Drawing.SystemIcons.Application,
            Text = Constants.Other.appTitle,
            Visible = true
        };
        static readonly HttpClient client = new();

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
            await CheckForUpdate();
            CheckStoredVariables();
            AddContextItems();
            CheckForSelfUpdate(cb_CheckSelfUpdate.Checked);
            UpdateFileAttributes(cb_HideConfig.Checked);
            Secret(true);
        }

        private void AddContextItems()
        {
            notifyIcon.ContextMenuStrip.Items.Add("Show", null, ShowWindowClicked);
            notifyIcon.ContextMenuStrip.Items.Add("Check for update", null, CheckUpdateClicked);
            notifyIcon.ContextMenuStrip.Items.Add("Exit", System.Drawing.SystemIcons.Error.ToBitmap(), ExitClicked);
        }

        private async void ShowWindow()
        {
            Show();
            WindowState = FormWindowState.Normal;

            if (cb_CheckUpateOnClick.Checked)
                await CheckForUpdate();
        }

        private async void CheckForSelfUpdate(bool checkForSelfUpdate)
        {
            if (!File.Exists(Constants.Paths.installPath))
                File.Copy(Constants.Paths.currentPath, Constants.Paths.installPath);

            else if (checkForSelfUpdate)
            {
                await Task.Delay(500);

                const string ChromiumUpdater = "ChromiumUpdaterGUI";
                GitHubClient githubClient = new(new ProductHeaderValue(ChromiumUpdater));
                System.Collections.Generic.IReadOnlyList<Release> releases = await githubClient.Repository.Release.GetAll("ozkut", ChromiumUpdater);
                Release latest = releases[0];

                float currentVersion = float.Parse(FileVersionInfo.GetVersionInfo(Constants.Paths.currentPath).ProductVersion);//version of currently running program
                float installedVersion = float.Parse(FileVersionInfo.GetVersionInfo(Constants.Paths.installPath).ProductVersion);
                float latestVersion = float.Parse(latest.Name.ToString());

                string currentVersionString = currentVersion.ToString("0.0");
                string installedVersionString = installedVersion.ToString("0.0");
                string latestVersionString = latestVersion.ToString("0.0");

                await Task.Delay(1000);
                if (installedVersion < currentVersion)
                {
                    DialogResult updateSelfResult =
                    MessageBox.Show($"The currently installed version of Chromium Updater (Version {installedVersionString}) is older than the version that is currently running! (Version {currentVersionString})\n" +
                                    "Would you like to replace the old version with the version currently running?",
                                    Constants.Other.appTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (updateSelfResult == DialogResult.Yes)
                        File.Copy(Constants.Paths.currentPath, Constants.Paths.installPath, true);
                }
                else if (installedVersion > currentVersion)
                {
                    MessageBox.Show($"A newer version (Version {installedVersionString}) of Chromium Updater is already installed on your system!",
                                    Constants.Other.appTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (latestVersion > currentVersion || latestVersion > installedVersion)
                {
                    DialogResult installLatestDialog =
                    MessageBox.Show($"A newer version of Chromium Updater is available! (Version {latestVersionString})\n" +
                                    $"Would you like yo update from Version {installedVersionString} that is currently running?",
                                    Constants.Other.appTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (installLatestDialog == DialogResult.Yes)
                        await Task.Run(() => UpdateSelf());
                }
            }
        }

        private async Task UpdateSelf()
        {
            notifyIcon.ShowBalloonTip(Constants.Other.notificationTimeout, Constants.Other.appTitle, "Updating Chromum Updater", ToolTipIcon.Info);
            try
            {
                byte[] file = await client.GetByteArrayAsync("https://github.com/ozkut/ChromiumUpdater/releases/latest/download/Chromium.Updater.exe");
                string fileLocation = Constants.Paths.installPath;
                if (File.Exists(fileLocation))
                    File.Delete(fileLocation);
                File.WriteAllBytes(fileLocation, file);
                notifyIcon.ShowBalloonTip(Constants.Other.notificationTimeout, Constants.Other.appTitle, "Update Complete!", ToolTipIcon.Info);
            }
            catch (Exception e) { DisplayErrorMessage("An error has occured when trying to update Chromium Updater.\n", e); }
        }

        //some events
        private void ShowWindowClicked(object sender, EventArgs e) => ShowWindow();

        private async void CheckUpdateClicked(object sender, EventArgs e)
        {
            ShowWindow();
            await CheckForUpdate();
        }

        private static void ExitClicked(object sender, EventArgs e) => ExitProgram();

        //storedVaribles stuff
        private async void UpdateCheckboxesFromFile()
        {
            UpdateFileAttributes(false);

            bool startOnBootState, checkUpdateOnClickState, hideConfigState, checkSelfUpdate;
            while (true)
            {
                try
                {
                    SerializeableVariables vars = JsonSerializer.Deserialize<SerializeableVariables>(await File.ReadAllTextAsync(Constants.StoredVariables.filePath));
                    startOnBootState = bool.Parse(vars.StartOnBoot);
                    checkUpdateOnClickState = bool.Parse(vars.CheckUpdateOnClick);
                    hideConfigState = bool.Parse(vars.HideConfig);
                    checkSelfUpdate = bool.Parse(vars.CheckForSelfUpdate);
                    break;
                }
                catch
                { UpdateStoredVariables(); }
            }

            cb_Startup.Checked = startOnBootState;
            cb_CheckUpateOnClick.Checked = checkUpdateOnClickState;
            cb_HideConfig.Checked = hideConfigState;
            cb_CheckSelfUpdate.Checked = checkSelfUpdate;

            UpdateFileAttributes(cb_HideConfig.Checked);
        }

        private void CheckStoredVariables()
        {
            if (File.Exists(Constants.StoredVariables.filePath))
                UpdateCheckboxesFromFile();
            else
            {
                Directory.CreateDirectory(Constants.StoredVariables.directory);
                UpdateStoredVariables();
            }
        }

        private async void UpdateStoredVariables()
        {
            SerializeableVariables vars = new()
            {
                StartOnBoot = cb_Startup.Checked.ToString(),
                CheckUpdateOnClick = cb_CheckUpateOnClick.Checked.ToString(),
                HideConfig = cb_HideConfig.Checked.ToString(),
                CheckForSelfUpdate = cb_CheckSelfUpdate.Checked.ToString()
            };
            await File.WriteAllTextAsync(Constants.StoredVariables.filePath, JsonSerializer.Serialize(vars, new JsonSerializerOptions { WriteIndented = true }));
            UpdateFileAttributes(cb_HideConfig.Checked);
        }

        private static void UpdateFileAttributes(bool hideFile)
        {
            if (File.Exists(Constants.StoredVariables.filePath))
            {
                FileAttributes attributes = File.GetAttributes(Constants.StoredVariables.filePath);
                if (hideFile)
                    File.SetAttributes(Constants.StoredVariables.filePath, attributes | FileAttributes.Hidden);
                else
                    File.SetAttributes(Constants.StoredVariables.filePath, attributes &= ~FileAttributes.Hidden);
            }
        }

        //the original methods
        private static void CheckStartupStatus(bool startOnBoot)
        {
            //inside HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run
            using Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            switch (startOnBoot)
            {
                case true:
                    regKey.SetValue(Constants.Other.appTitle, Constants.Paths.installPath);
                    break;

                case false:
                    regKey.DeleteValue(Constants.Other.appTitle, false);
                    break;
            }
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
                    DownloadNewestVersion().Wait();

                await Task.Run(() => CheckForUpdate());
            }
            catch (Exception e)
            {
                DisplayErrorMessage("An unknown error has occured.\n" +
                                    e.Message.ToString(), e);
            }

            //don't know if this is necessary or if i just just hard-code the "newest version:" and "current version:"
            //into the strings rather than adding the version numbers on top and removing the extras every time
            if (l_CurrentVersion.Text.Length > currentVersion.Length || l_NewestVersion.Text.Length > newestVersion.Length)
            {
                l_CurrentVersion.Text = l_CurrentVersion.Text.Trim(currentVersion.ToCharArray());
                l_NewestVersion.Text = l_NewestVersion.Text.Trim(newestVersion.ToCharArray());
            }

            l_CurrentVersion.Text += currentVersion;
            l_NewestVersion.Text += newestVersion;

            if (hasInternet)
            {
                if (currentVersion == newestVersion)
                    IsUpToDate(Visible);

                else
                {
                    if (!Visible)
                        notifyIcon.ShowBalloonTip(Constants.Other.notificationTimeout, Constants.Other.appTitle, "An update is available!", ToolTipIcon.None);

                    else
                    {
                        DialogResult updateAvailableDialog =
                        MessageBox.Show($"Current Version: {currentVersion}\n" +
                                        $"Newest version available: {newestVersion}\n" +
                                        "Would you like to update?",
                                        Constants.Other.appTitle,
                                        MessageBoxButtons.YesNo,
                                        MessageBoxIcon.Question);

                        if (updateAvailableDialog == DialogResult.Yes)
                            await DownloadNewestVersion();
                    }
                }
            }
        }

        //i can *probably* modify this so it can download both this program and chromium, but i'm too lazy
        private async Task DownloadNewestVersion()
        {
            notifyIcon.ShowBalloonTip(Constants.Other.notificationTimeout, Constants.Other.appTitle, "Downloading update file", ToolTipIcon.Info);

            byte[] file;
            string fileLocation = string.Empty;
            string trimmedFileLocation = string.Empty;
            const string fileName = "mini_installer.sync.exe";

            try
            {
                file = await client.GetByteArrayAsync("https://github.com/Hibbiki/chromium-win64/releases/latest/download/mini_installer.sync.exe");
                fileLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
                trimmedFileLocation = fileLocation.Trim(fileName.ToCharArray());
                File.WriteAllBytes(fileLocation, file);
            }
            catch (HttpRequestException e)
            {
                DisplayErrorMessage("An HTTP error has occured when trying to download the latest version of Chromium.\n" +
                                    "Please check your internet connection.", new Exception(e.Message));
            }
            catch (PathTooLongException e)
            {
                DisplayErrorMessage($"The path '{fileLocation}' is too long.\n"/* +
                                    "Please enter a shorter file path."*/, e);
                //second part will be used when the option to sellect a custom location is added
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
            catch (IOException e)
            {
                DisplayErrorMessage("An I/O error occured when trying to write the upadte file to " +
                                    $"'{trimmedFileLocation}'\n" +
                                    "Please make sure that the directory isn't read-only.", e);
            }
            catch (Exception e)
            {
                DisplayErrorMessage("An unknown error has occured.\n" +
                                    e.Message.ToString(), e);
            }

            notifyIcon.ShowBalloonTip(Constants.Other.notificationTimeout, Constants.Other.appTitle, "File downloaded succesfully", ToolTipIcon.Info);

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
            InstallUpdate(fileLocation, Visible);

            DialogResult deleteDialog =
            MessageBox.Show($"Would you like to delete the update file from '{fileLocation}'?",
                            Constants.Other.appTitle,
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

            if (deleteDialog == DialogResult.Yes)
                File.Delete(fileLocation);
        }

        private static void IsUpToDate(bool isVisible)
        {
            const string upToDateText = "Chromium is up-to-date!";

            if (!isVisible)
                notifyIcon.ShowBalloonTip(Constants.Other.notificationTimeout, Constants.Other.appTitle, upToDateText, ToolTipIcon.None);

            else
                MessageBox.Show(upToDateText, Constants.Other.appTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static void InstallUpdate(string fileLocation, bool isVisible)
        {
            notifyIcon.ShowBalloonTip(Constants.Other.notificationTimeout, Constants.Other.appTitle, "Installing", ToolTipIcon.Info);

            Process.Start(fileLocation).WaitForExit();

            notifyIcon.ShowBalloonTip(Constants.Other.notificationTimeout, Constants.Other.appTitle, "Install Complete!", ToolTipIcon.Info);

            if (isVisible)
            {
                System.Threading.Thread.Sleep(750);
                MessageBox.Show("Install Complete!", Constants.Other.appTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        }

        private void cb_HideConfig_CheckedChanged(object sender, EventArgs e)
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
                if (File.Exists(Constants.StoredVariables.filePath))
                    File.Delete(Constants.StoredVariables.filePath);
            }
        }

        //program end stuff
        private void Form_Closing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing || e.CloseReason == CloseReason.None)
            {
                Hide();
                e.Cancel = true;
                notifyIcon.ShowBalloonTip(Constants.Other.notificationTimeout, Constants.Other.appTitle, "Minimized to system tray", ToolTipIcon.None);
            }
        }

        private void DisplayErrorMessage(string errorMessage, Exception e)
        {
            if (e is HttpRequestException)
            {
                if (!Visible)
                    notifyIcon.ShowBalloonTip(Constants.Other.notificationTimeout, Constants.Other.appTitle, errorMessage, ToolTipIcon.Error);
                else
                    MessageBox.Show(errorMessage, Constants.Other.appTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

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

        private static void ExitProgram()
        {
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
            client.Dispose();
            Environment.Exit(0);
        }

        private async static void Secret(bool enabled)
        {
            if (enabled && DateTime.Today.ToString("M") == "28 March")
            {
                notifyIcon.ShowBalloonTip(Constants.Other.notificationTimeout, Constants.Other.appTitle, "Happy birthday to me!", ToolTipIcon.None);
                System.Threading.Thread.Sleep(500);
                await Task.Run(() => Process.Start(new ProcessStartInfo("cmd", $"/c start " + "https://www.youtube.com/watch?v=dQw4w9WgXcQ".Replace("&", "^&")) { CreateNoWindow = true }));
                MessageBox.Show("Happy birthday to me!", Constants.Other.appTitle, MessageBoxButtons.OK, MessageBoxIcon.None);
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