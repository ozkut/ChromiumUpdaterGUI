using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

//Chromium Updater made by Ozkut
//This is the GUI version of Chromium Updater

namespace ChromiumUpdaterGUI
{
    public partial class Form : System.Windows.Forms.Form
    {
        //initilize variables
        static NotifyIcon notifyIcon = new() { Visible = true, Icon = System.Drawing.SystemIcons.Application };
        HttpClient client = new();

        //initilize constants
        const string appTitle = "Chromium Updater";
        const string storedVariables = "ChromiumUpdater_StoredVariables.txt";

        const int notificationTimeout = 3000;

        const string StartOnBoot = "StartOnBoot=";
        const string CheckUpdateOnClick = "CheckUpdateOnClick=";

        public Form() => InitializeComponent();

        private async void Form1_Load(object sender, EventArgs e) 
        {
            Hide();
            await CheckForUpdate();

            if (File.Exists(storedVariables))
            {
                UpdateCheckboxesFromFile();
                if (cb_Startup.Checked)
                    CheckStartupStatus(cb_Startup.Checked);
            }
                
            else
                CreateStoredVariables();

            notifyIcon.BalloonTipClicked += NotifyIconClicked;
            notifyIcon.Click += NotifyIconClicked;
        }

        private async void NotifyIconClicked(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;

            if (cb_CheckUpateOnClick.Checked)
                await CheckForUpdate();
        }

        private void UpdateCheckboxesFromFile()
        {
            UpdateFileAttributes(false);

            const string True = "True";
            const string False = "False";

            foreach (string line in File.ReadAllLines(storedVariables))
            {
                switch (line)
                {
                    case StartOnBoot + True:
                        cb_Startup.Checked = true;
                        break;

                    case StartOnBoot + False:
                        cb_Startup.Checked = false;
                        break;

                    case CheckUpdateOnClick + True:
                        cb_CheckUpateOnClick.Checked = true;
                        break;

                    case CheckUpdateOnClick + False:
                        cb_CheckUpateOnClick.Checked = false;
                        break;
                }
            }

            UpdateFileAttributes(true);
        }

        //storedVaribles stuff
        private void CreateStoredVariables()
        {
            File.WriteAllText(storedVariables,
                              StartOnBoot + cb_Startup.Checked.ToString() + "\n" +
                              CheckUpdateOnClick + cb_CheckUpateOnClick.Checked.ToString() + "\n");

            UpdateFileAttributes(true);
        }

        private async void UpdateStoredVariables()
        {
            if (File.Exists(storedVariables))
            {
                UpdateFileAttributes(false);

                string[] temp_StoredVariables = File.ReadAllLines(storedVariables);
                string[] final_StoredVariables = new string[temp_StoredVariables.Length];

                for (int i = 0; i < temp_StoredVariables.Length; i++)
                {
                    if (temp_StoredVariables[i] == $"{StartOnBoot}True" || temp_StoredVariables[i] == $"{StartOnBoot}False")
                        final_StoredVariables[i] = StartOnBoot + cb_Startup.Checked;

                    else if (temp_StoredVariables[i] == $"{CheckUpdateOnClick}True" || temp_StoredVariables[i] == $"{CheckUpdateOnClick}False")
                        final_StoredVariables[i] = CheckUpdateOnClick + cb_CheckUpateOnClick.Checked;

                    else
                    {
                        switch (i)
                        {
                            case 0:
                                final_StoredVariables[i] = StartOnBoot + cb_Startup.Checked;
                                break;

                            case 1:
                                final_StoredVariables[i] = CheckUpdateOnClick + cb_CheckUpateOnClick.Checked;
                                break;
                        }
                    }

                    for (int j = 0; j < final_StoredVariables.Length; j++)
                    {
                        if (j == 0)
                            await File.WriteAllTextAsync(storedVariables, final_StoredVariables[j] + "\n");//throws access denied exception
                        else
                            await File.AppendAllTextAsync(storedVariables, final_StoredVariables[j] + "\n");
                    }
                }
                UpdateFileAttributes(true);
            }

            else
                CreateStoredVariables();
        }

        private static void UpdateFileAttributes(bool hideFile)
        {
            FileAttributes attributes = File.GetAttributes(storedVariables);

            if (hideFile)
                File.SetAttributes(storedVariables, attributes | FileAttributes.Hidden);
            else
            {
                attributes &= ~FileAttributes.Hidden;
                File.SetAttributes(storedVariables, attributes);
            }
        }

        //the original methods
        private static void CheckStartupStatus(bool startOnBoot)
        {
            string programPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), appTitle.Replace(' ', '.') + ".exe");
            string currentProgramPath = Process.GetCurrentProcess().MainModule.FileName;

            switch (startOnBoot)
            {
                case true:
                    if (File.Exists(programPath))
                        File.Replace(currentProgramPath, programPath, null);
                    else
                        File.Copy(currentProgramPath, programPath);
                    break;

                case false:
                    File.Delete(programPath);
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
            catch (FileNotFoundException e) 
            { 
                DisplayErrorMessage($"A valid install of Chromium could not be found at {Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Chromium\Application\chrome.exe")}\n" +
                                    "Please make sure that Chromium is installed at thet locatiom.", e); 
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
                        notifyIcon.ShowBalloonTip(notificationTimeout, appTitle, "An update is available!", ToolTipIcon.None);

                    else
                    {
                        DialogResult updateAvailableDialog =
                        MessageBox.Show($"Current Version: {currentVersion}\n" +
                                        $"Newest version available: {newestVersion}\n" +
                                        "Would you like to update?",
                                        appTitle,
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
            notifyIcon.ShowBalloonTip(notificationTimeout, appTitle, "Downloading update file", ToolTipIcon.Info);

            byte[] file;
            string fileLocation = string.Empty;
            const string fileName = "mini_installer.sync.exe";

            try
            {
                file = await client.GetByteArrayAsync("https://github.com/Hibbiki/chromium-win64/releases/latest/download/mini_installer.sync.exe");
                fileLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
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
                                    $"'{fileLocation.Trim(fileName.ToCharArray())}'\n" +
                                    "Please check if the program has the correct permissions to write to the given directory.", e);
            }
            catch (DirectoryNotFoundException e)
            {
                DisplayErrorMessage($"The directory '{fileLocation.Trim(fileName.ToCharArray())}' could not be found.\n" +
                                    "Please make sure that the directory exists.", e);
            }
            catch (IOException e) 
            {
                DisplayErrorMessage("An I/O error occured when trying to write the upadte file to " +
                                    $"'{fileLocation.Trim(fileName.ToCharArray())}'\n" +
                                    "Please make sure that the directory isn't read-only.", e);
            }
            catch (Exception e) 
            {
                DisplayErrorMessage("An unknown error has occured.\n" + 
                                    e.Message.ToString(), e);
            }

            notifyIcon.ShowBalloonTip(notificationTimeout, appTitle, "Update file downloaded succesfully", ToolTipIcon.Info);

            Process[] processes = Process.GetProcessesByName("chrome");

            if (processes.Length > 0)
            {
                DialogResult browserOpenDialog =
                MessageBox.Show("An instance of Chromium is already running!\n" +
                                "Would you like to close it?",
                                appTitle,
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Exclamation);

                if (browserOpenDialog == DialogResult.Yes)
                {
                    foreach (Process process in processes)
                        process.Kill();
                }

                DoUpdate(fileLocation, Visible);
            }

            else
                DoUpdate(fileLocation, Visible);

            DialogResult deleteDialog =
            MessageBox.Show($"Would you like to delete the update file from {fileLocation}?",
                            appTitle,
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

            if (deleteDialog == DialogResult.Yes)
                File.Delete(fileLocation);
        }

        private static void IsUpToDate(bool isVisible)
        {
            const string upToDateText = "Chromium is up-to-date!";

            if (!isVisible)
                notifyIcon.ShowBalloonTip(notificationTimeout, appTitle, upToDateText, ToolTipIcon.None);

            else
                MessageBox.Show(upToDateText, appTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static void DoUpdate(string fileLocation, bool isVisible)
        {
            notifyIcon.ShowBalloonTip(notificationTimeout, appTitle, "Installing Update", ToolTipIcon.Info);

            Process updater = Process.Start(fileLocation);
            updater.WaitForExit();

            notifyIcon.ShowBalloonTip(notificationTimeout, appTitle, "Update Complete!", ToolTipIcon.Info);
            
            if (isVisible)
            {
                System.Threading.Thread.Sleep(750);
                MessageBox.Show("Update Complete!", appTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //checkboxes
        private void cb_Startup_CheckedChanged(object sender, EventArgs e) 
        { 
            UpdateStoredVariables();
            CheckStartupStatus(cb_Startup.Checked);
        }

        private void cb_CheckUpateOnClick_CheckedChanged(object sender, EventArgs e) => UpdateStoredVariables();

        //buttons
        private async void b_CheckUpdate_Click(object sender, EventArgs e) => await CheckForUpdate();

        private void b_Exit_Click(object sender, EventArgs e) => ExitProgram();

        //program end stuff
        private void Form_Closing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing || e.CloseReason == CloseReason.None)
            {
                Hide();
                e.Cancel = true;
            }
        }

        private void DisplayErrorMessage(string errorMessage, Exception e)
        {
            if (e is HttpRequestException)
            {
                if (!Visible)
                    notifyIcon.ShowBalloonTip(notificationTimeout, appTitle, errorMessage, ToolTipIcon.Warning);
                else
                    MessageBox.Show(errorMessage, appTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;
            }  

            string errorLogLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ChromiumUpdater_Error.Log");
            MessageBox.Show("The program has crashed with the following exception:\n" + errorMessage + 
                            $"\nAn error log has been created at: '{errorLogLocation}'", 
                            appTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);

            File.WriteAllText(errorLogLocation, e.ToString());
            Environment.Exit(1);
        }

        private void ExitProgram()
        {
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
            client.Dispose();
            Environment.Exit(0);
        }
    }
}