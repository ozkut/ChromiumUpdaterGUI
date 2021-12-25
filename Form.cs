using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Text.Json;
using Microsoft.Win32;

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
            Text = Constants.appTitle,
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

        //some events
        private void ShowWindowClicked(object sender, EventArgs e) => ShowWindow();

        private async void CheckUpdateClicked(object sender, EventArgs e)
        {
            ShowWindow();
            await CheckForUpdate();
        }

        private static void ExitClicked(object sender, EventArgs e) => ExitProgram();

        //storedVaribles stuff
        private void UpdateCheckboxesFromFile()
        {
            UpdateFileAttributes(false);

            SerializeableVariables vars = JsonSerializer.Deserialize<SerializeableVariables>(File.ReadAllText(Constants.storedVariables));
            bool startOnBootState, checkUpdateOnClickState, hideConfigState;
            
            while (true)
            {
                try
                {
                    startOnBootState = bool.Parse(vars.StartOnBoot);
                    checkUpdateOnClickState = bool.Parse(vars.CheckUpdateOnClick);
                    hideConfigState = bool.Parse(vars.HideConfig);
                    break;
                }
                catch (FormatException)
                {
                    UpdateStoredVariables();
                }
            }

            cb_Startup.Checked = startOnBootState;
            cb_CheckUpateOnClick.Checked = checkUpdateOnClickState;
            cb_HideConfig.Checked = hideConfigState;

            UpdateFileAttributes(cb_HideConfig.Checked);
        }

        private void CheckStoredVariables()
        {
            if (File.Exists(Constants.storedVariables))
            {
                UpdateCheckboxesFromFile();
                if (cb_Startup.Checked)
                    CheckStartupStatus(cb_Startup.Checked);
            }
            else
            {
                Directory.CreateDirectory(Constants.storedVariablesDirectory);
                UpdateStoredVariables();
            }
        }

        private async void UpdateStoredVariables()
        {
            SerializeableVariables vars = new()
            {
                StartOnBoot = cb_Startup.Checked.ToString(),
                CheckUpdateOnClick = cb_CheckUpateOnClick.Checked.ToString(),
                HideConfig = cb_HideConfig.Checked.ToString()
            };
            await File.WriteAllTextAsync(Constants.storedVariables, JsonSerializer.Serialize(vars, new JsonSerializerOptions { WriteIndented = true }));
            UpdateFileAttributes(cb_HideConfig.Checked);
        }

        private static void UpdateFileAttributes(bool hideFile)
        {
            if (File.Exists(Constants.storedVariables))
            {
                FileAttributes attributes = File.GetAttributes(Constants.storedVariables);
                if (hideFile)
                    File.SetAttributes(Constants.storedVariables, attributes | FileAttributes.Hidden);
                else
                    File.SetAttributes(Constants.storedVariables, attributes &= ~FileAttributes.Hidden);
            }
        }

        //the original methods
        private static void CheckStartupStatus(bool startOnBoot)
        {
            string currentProgramPath = Process.GetCurrentProcess().MainModule.FileName;
            string actualLocation = Path.Combine(Constants.storedVariablesDirectory, Constants.appTitle + ".exe");

            if (!File.Exists(actualLocation))
                File.Copy(currentProgramPath, actualLocation);

            //inside HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            switch (startOnBoot)
            {
                case true:
                    regKey.SetValue(Constants.appTitle, actualLocation);
                    break;

                case false:
                    regKey.DeleteValue(Constants.appTitle, false);
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
                                Constants.appTitle,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);

                DialogResult installResult =
                MessageBox.Show("Would you like to install Chromium?",
                                Constants.appTitle,
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question);

                if (installResult == DialogResult.Yes)
                        DownloadNewestVersion().Wait();
                
                ExitProgram();
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
                        notifyIcon.ShowBalloonTip(Constants.notificationTimeout, Constants.appTitle, "An update is available!", ToolTipIcon.None);

                    else
                    {
                        DialogResult updateAvailableDialog =
                        MessageBox.Show($"Current Version: {currentVersion}\n" +
                                        $"Newest version available: {newestVersion}\n" +
                                        "Would you like to update?",
                                        Constants.appTitle,
                                        MessageBoxButtons.YesNo,
                                        MessageBoxIcon.Question);

                        if (updateAvailableDialog == DialogResult.Yes)
                            await DownloadNewestVersion();
                    }
                }
            }
        }

        private async Task DownloadNewestVersion()
        {
            notifyIcon.ShowBalloonTip(Constants.notificationTimeout, Constants.appTitle, "Downloading update file", ToolTipIcon.Info);

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

            notifyIcon.ShowBalloonTip(Constants.notificationTimeout, Constants.appTitle, "File downloaded succesfully", ToolTipIcon.Info);

            Process[] processes = Process.GetProcessesByName("chrome");

            if (processes.Length > 0)
            {
                DialogResult browserOpenDialog =
                MessageBox.Show("An instance of Chromium is already running!\n" +
                                "Would you like to close it?",
                                Constants.appTitle,
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
                            Constants.appTitle,
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

            if (deleteDialog == DialogResult.Yes)
                File.Delete(fileLocation);
        }

        private static void IsUpToDate(bool isVisible)
        {
            const string upToDateText = "Chromium is up-to-date!";

            if (!isVisible)
                notifyIcon.ShowBalloonTip(Constants.notificationTimeout, Constants.appTitle, upToDateText, ToolTipIcon.None);

            else
                MessageBox.Show(upToDateText, Constants.appTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static void InstallUpdate(string fileLocation, bool isVisible)
        {
            notifyIcon.ShowBalloonTip(Constants.notificationTimeout, Constants.appTitle, "Installing", ToolTipIcon.Info);

            Process.Start(fileLocation).WaitForExit();

            notifyIcon.ShowBalloonTip(Constants.notificationTimeout, Constants.appTitle, "Install Complete!", ToolTipIcon.Info);
            
            if (isVisible)
            {
                System.Threading.Thread.Sleep(750);
                MessageBox.Show("Install Complete!", Constants.appTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            Constants.appTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (deleteConfigResult == DialogResult.Yes)
            {
                UpdateFileAttributes(false);
                if (File.Exists(Constants.storedVariables))
                    File.Delete(Constants.storedVariables);
            }
        }

        //program end stuff
        private void Form_Closing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing || e.CloseReason == CloseReason.None)
            {
                Hide();
                e.Cancel = true;
                notifyIcon.ShowBalloonTip(Constants.notificationTimeout, Constants.appTitle, "Minimized to system tray", ToolTipIcon.None);
            }
        }

        private void DisplayErrorMessage(string errorMessage, Exception e)
        {
            if (e is HttpRequestException)
            {
                if (!Visible)
                    notifyIcon.ShowBalloonTip(Constants.notificationTimeout, Constants.appTitle, errorMessage, ToolTipIcon.Error);
                else
                    MessageBox.Show(errorMessage, Constants.appTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }  

            string errorLogLocation = Path.Combine(Constants.storedVariablesDirectory, "ChromiumUpdater_Error.Log");
            MessageBox.Show("The program has crashed with the following exception:\n" + errorMessage + 
                            $"\nAn error log has been created at: '{errorLogLocation}'",
                            Constants.appTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);

            if (!Directory.Exists(Constants.storedVariablesDirectory))
                Directory.CreateDirectory(Constants.storedVariablesDirectory);

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
                notifyIcon.ShowBalloonTip(Constants.notificationTimeout, Constants.appTitle, "Happy birthday to me!", ToolTipIcon.None);
                System.Threading.Thread.Sleep(500);
                await Task.Run(() => Process.Start(new ProcessStartInfo("cmd", $"/c start " + "https://www.youtube.com/watch?v=dQw4w9WgXcQ".Replace("&", "^&")) { CreateNoWindow = true }));
                MessageBox.Show("Happy birthday to me!", Constants.appTitle, MessageBoxButtons.OK, MessageBoxIcon.None);
            }
        }
    }
}
////some very VERY bad code
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
