
namespace ChromiumUpdaterGUI
{
    partial class Form
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.b_CheckUpdate = new System.Windows.Forms.Button();
            this.l_CurrentVersion = new System.Windows.Forms.Label();
            this.l_NewestVersion = new System.Windows.Forms.Label();
            this.cb_Startup = new System.Windows.Forms.CheckBox();
            this.b_Exit = new System.Windows.Forms.Button();
            this.cb_CheckUpateOnClick = new System.Windows.Forms.CheckBox();
            this.cb_HideConfig = new System.Windows.Forms.CheckBox();
            this.b_DeleteConfig = new System.Windows.Forms.Button();
            this.cb_CheckSelfUpdate = new System.Windows.Forms.CheckBox();
            this.b_SelfUpdateCheck = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.numList = new System.Windows.Forms.NumericUpDown();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.l_Progress = new System.Windows.Forms.Label();
            this.b_CancelDownload = new System.Windows.Forms.Button();
            this.l_DownloadAmount = new System.Windows.Forms.Label();
            this.l_DownloadSpeed = new System.Windows.Forms.Label();
            this.cb_EnableRegularChecks = new System.Windows.Forms.CheckBox();
            this.comboBox_RegularUpdateInterval = new System.Windows.Forms.ComboBox();
            this.cb_ShowUpToDateNotif = new System.Windows.Forms.CheckBox();
            this.b_OpenInstallFolder = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numList)).BeginInit();
            this.SuspendLayout();
            // 
            // b_CheckUpdate
            // 
            this.b_CheckUpdate.Location = new System.Drawing.Point(159, 199);
            this.b_CheckUpdate.Name = "b_CheckUpdate";
            this.b_CheckUpdate.Size = new System.Drawing.Size(186, 25);
            this.b_CheckUpdate.TabIndex = 6;
            this.b_CheckUpdate.Text = "Check for Chromium updates";
            this.b_CheckUpdate.UseVisualStyleBackColor = true;
            this.b_CheckUpdate.Click += new System.EventHandler(this.b_CheckUpdate_Click);
            // 
            // l_CurrentVersion
            // 
            this.l_CurrentVersion.AutoSize = true;
            this.l_CurrentVersion.Location = new System.Drawing.Point(257, 12);
            this.l_CurrentVersion.Name = "l_CurrentVersion";
            this.l_CurrentVersion.Size = new System.Drawing.Size(105, 17);
            this.l_CurrentVersion.TabIndex = 1;
            this.l_CurrentVersion.Text = "Current Version: ";
            // 
            // l_NewestVersion
            // 
            this.l_NewestVersion.AutoSize = true;
            this.l_NewestVersion.Location = new System.Drawing.Point(257, 39);
            this.l_NewestVersion.Name = "l_NewestVersion";
            this.l_NewestVersion.Size = new System.Drawing.Size(105, 17);
            this.l_NewestVersion.TabIndex = 2;
            this.l_NewestVersion.Text = "Newest Version: ";
            // 
            // cb_Startup
            // 
            this.cb_Startup.AutoSize = true;
            this.cb_Startup.Location = new System.Drawing.Point(12, 12);
            this.cb_Startup.Name = "cb_Startup";
            this.cb_Startup.Size = new System.Drawing.Size(105, 21);
            this.cb_Startup.TabIndex = 1;
            this.cb_Startup.Text = "Start on boot";
            this.cb_Startup.UseVisualStyleBackColor = true;
            this.cb_Startup.CheckedChanged += new System.EventHandler(this.cb_Startup_CheckedChanged);
            // 
            // b_Exit
            // 
            this.b_Exit.Location = new System.Drawing.Point(351, 199);
            this.b_Exit.Name = "b_Exit";
            this.b_Exit.Size = new System.Drawing.Size(79, 25);
            this.b_Exit.TabIndex = 0;
            this.b_Exit.Text = "Exit";
            this.b_Exit.UseVisualStyleBackColor = true;
            this.b_Exit.Click += new System.EventHandler(this.b_Exit_Click);
            // 
            // cb_CheckUpateOnClick
            // 
            this.cb_CheckUpateOnClick.AutoSize = true;
            this.cb_CheckUpateOnClick.Checked = true;
            this.cb_CheckUpateOnClick.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_CheckUpateOnClick.Location = new System.Drawing.Point(12, 39);
            this.cb_CheckUpateOnClick.Name = "cb_CheckUpateOnClick";
            this.cb_CheckUpateOnClick.Size = new System.Drawing.Size(210, 21);
            this.cb_CheckUpateOnClick.TabIndex = 2;
            this.cb_CheckUpateOnClick.Text = "Check for updates on maximise";
            this.cb_CheckUpateOnClick.UseVisualStyleBackColor = true;
            this.cb_CheckUpateOnClick.CheckedChanged += new System.EventHandler(this.cb_CheckUpateOnClick_CheckedChanged);
            // 
            // cb_HideConfig
            // 
            this.cb_HideConfig.AutoSize = true;
            this.cb_HideConfig.Checked = true;
            this.cb_HideConfig.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_HideConfig.Location = new System.Drawing.Point(12, 66);
            this.cb_HideConfig.Name = "cb_HideConfig";
            this.cb_HideConfig.Size = new System.Drawing.Size(115, 21);
            this.cb_HideConfig.TabIndex = 3;
            this.cb_HideConfig.Text = "Hide config file";
            this.cb_HideConfig.UseVisualStyleBackColor = true;
            this.cb_HideConfig.CheckedChanged += new System.EventHandler(this.cb_HideConfig_CheckedChanged);
            // 
            // b_DeleteConfig
            // 
            this.b_DeleteConfig.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.b_DeleteConfig.Location = new System.Drawing.Point(335, 172);
            this.b_DeleteConfig.Name = "b_DeleteConfig";
            this.b_DeleteConfig.Size = new System.Drawing.Size(95, 24);
            this.b_DeleteConfig.TabIndex = 8;
            this.b_DeleteConfig.Text = "Delete Config";
            this.b_DeleteConfig.UseVisualStyleBackColor = true;
            this.b_DeleteConfig.Click += new System.EventHandler(this.b_DeleteConfig_Click);
            // 
            // cb_CheckSelfUpdate
            // 
            this.cb_CheckSelfUpdate.AutoSize = true;
            this.cb_CheckSelfUpdate.Checked = true;
            this.cb_CheckSelfUpdate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_CheckSelfUpdate.Location = new System.Drawing.Point(12, 93);
            this.cb_CheckSelfUpdate.Name = "cb_CheckSelfUpdate";
            this.cb_CheckSelfUpdate.Size = new System.Drawing.Size(151, 21);
            this.cb_CheckSelfUpdate.TabIndex = 4;
            this.cb_CheckSelfUpdate.Text = "Check for self update";
            this.cb_CheckSelfUpdate.UseVisualStyleBackColor = true;
            this.cb_CheckSelfUpdate.CheckedChanged += new System.EventHandler(this.cb_CheckSelfUpdate_CheckedChanged);
            // 
            // b_SelfUpdateCheck
            // 
            this.b_SelfUpdateCheck.Location = new System.Drawing.Point(12, 199);
            this.b_SelfUpdateCheck.Name = "b_SelfUpdateCheck";
            this.b_SelfUpdateCheck.Size = new System.Drawing.Size(141, 25);
            this.b_SelfUpdateCheck.TabIndex = 5;
            this.b_SelfUpdateCheck.Text = "Check for self update";
            this.b_SelfUpdateCheck.Click += new System.EventHandler(this.CheckSelfUpdateClicked);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 175);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 17);
            this.label1.TabIndex = 5;
            this.label1.Text = "Timeout:";
            // 
            // numList
            // 
            this.numList.InterceptArrowKeys = false;
            this.numList.Location = new System.Drawing.Point(66, 171);
            this.numList.Name = "numList";
            this.numList.Size = new System.Drawing.Size(27, 25);
            this.numList.TabIndex = 7;
            this.numList.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(257, 59);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(132, 17);
            this.progressBar.TabIndex = 4;
            this.progressBar.Visible = false;
            // 
            // l_Progress
            // 
            this.l_Progress.AutoSize = true;
            this.l_Progress.Location = new System.Drawing.Point(390, 59);
            this.l_Progress.Name = "l_Progress";
            this.l_Progress.Size = new System.Drawing.Size(40, 17);
            this.l_Progress.TabIndex = 3;
            this.l_Progress.Text = "000%";
            this.l_Progress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.l_Progress.Visible = false;
            // 
            // b_CancelDownload
            // 
            this.b_CancelDownload.Location = new System.Drawing.Point(304, 99);
            this.b_CancelDownload.Name = "b_CancelDownload";
            this.b_CancelDownload.Size = new System.Drawing.Size(126, 25);
            this.b_CancelDownload.TabIndex = 9;
            this.b_CancelDownload.Text = "Cancel download";
            this.b_CancelDownload.UseVisualStyleBackColor = true;
            this.b_CancelDownload.Visible = false;
            this.b_CancelDownload.Click += new System.EventHandler(this.b_CancelDownload_Click);
            // 
            // l_DownloadAmount
            // 
            this.l_DownloadAmount.AutoSize = true;
            this.l_DownloadAmount.Location = new System.Drawing.Point(257, 79);
            this.l_DownloadAmount.Name = "l_DownloadAmount";
            this.l_DownloadAmount.Size = new System.Drawing.Size(109, 17);
            this.l_DownloadAmount.TabIndex = 10;
            this.l_DownloadAmount.Text = "000 MB / 000 MB";
            this.l_DownloadAmount.Visible = false;
            // 
            // l_DownloadSpeed
            // 
            this.l_DownloadSpeed.AutoSize = true;
            this.l_DownloadSpeed.Location = new System.Drawing.Point(364, 79);
            this.l_DownloadSpeed.Name = "l_DownloadSpeed";
            this.l_DownloadSpeed.Size = new System.Drawing.Size(66, 17);
            this.l_DownloadSpeed.TabIndex = 11;
            this.l_DownloadSpeed.Text = "0000 KB/s";
            this.l_DownloadSpeed.Visible = false;
            // 
            // cb_EnableRegularChecks
            // 
            this.cb_EnableRegularChecks.AutoSize = true;
            this.cb_EnableRegularChecks.Checked = true;
            this.cb_EnableRegularChecks.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_EnableRegularChecks.Location = new System.Drawing.Point(12, 121);
            this.cb_EnableRegularChecks.Name = "cb_EnableRegularChecks";
            this.cb_EnableRegularChecks.Size = new System.Drawing.Size(293, 21);
            this.cb_EnableRegularChecks.TabIndex = 12;
            this.cb_EnableRegularChecks.Text = "Check for updates regularly             minute(s)";
            this.cb_EnableRegularChecks.UseVisualStyleBackColor = true;
            this.cb_EnableRegularChecks.CheckedChanged += new System.EventHandler(this.cb_EnableRegularChecks_CheckedChanged);
            // 
            // comboBox_RegularUpdateInterval
            // 
            this.comboBox_RegularUpdateInterval.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_RegularUpdateInterval.FormattingEnabled = true;
            this.comboBox_RegularUpdateInterval.Location = new System.Drawing.Point(197, 119);
            this.comboBox_RegularUpdateInterval.MaxDropDownItems = 5;
            this.comboBox_RegularUpdateInterval.Name = "comboBox_RegularUpdateInterval";
            this.comboBox_RegularUpdateInterval.Size = new System.Drawing.Size(44, 25);
            this.comboBox_RegularUpdateInterval.TabIndex = 13;
            this.comboBox_RegularUpdateInterval.SelectedIndexChanged += new System.EventHandler(this.comboBox_RegularUpdateInterval_SelectedIndexChanged);
            // 
            // cb_ShowUpToDateNotif
            // 
            this.cb_ShowUpToDateNotif.AutoSize = true;
            this.cb_ShowUpToDateNotif.Location = new System.Drawing.Point(12, 148);
            this.cb_ShowUpToDateNotif.Name = "cb_ShowUpToDateNotif";
            this.cb_ShowUpToDateNotif.Size = new System.Drawing.Size(304, 21);
            this.cb_ShowUpToDateNotif.TabIndex = 14;
            this.cb_ShowUpToDateNotif.Text = "Show notification when Chromium is up-to-date";
            this.cb_ShowUpToDateNotif.UseVisualStyleBackColor = true;
            this.cb_ShowUpToDateNotif.CheckedChanged += new System.EventHandler(this.cb_ShowUpToDateNotif_CheckedChanged);
            // 
            // b_OpenInstallFolder
            // 
            this.b_OpenInstallFolder.Location = new System.Drawing.Point(197, 172);
            this.b_OpenInstallFolder.Name = "b_OpenInstallFolder";
            this.b_OpenInstallFolder.Size = new System.Drawing.Size(135, 24);
            this.b_OpenInstallFolder.TabIndex = 15;
            this.b_OpenInstallFolder.Text = "Open install folder";
            this.b_OpenInstallFolder.UseVisualStyleBackColor = true;
            this.b_OpenInstallFolder.Click += new System.EventHandler(this.b_OpenInstallFolder_Click);
            // 
            // Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(442, 231);
            this.Controls.Add(this.b_OpenInstallFolder);
            this.Controls.Add(this.cb_ShowUpToDateNotif);
            this.Controls.Add(this.comboBox_RegularUpdateInterval);
            this.Controls.Add(this.cb_EnableRegularChecks);
            this.Controls.Add(this.l_DownloadSpeed);
            this.Controls.Add(this.l_DownloadAmount);
            this.Controls.Add(this.b_CancelDownload);
            this.Controls.Add(this.l_Progress);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.numList);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.b_SelfUpdateCheck);
            this.Controls.Add(this.cb_CheckSelfUpdate);
            this.Controls.Add(this.b_DeleteConfig);
            this.Controls.Add(this.cb_HideConfig);
            this.Controls.Add(this.cb_CheckUpateOnClick);
            this.Controls.Add(this.b_Exit);
            this.Controls.Add(this.cb_Startup);
            this.Controls.Add(this.l_NewestVersion);
            this.Controls.Add(this.l_CurrentVersion);
            this.Controls.Add(this.b_CheckUpdate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chromium Updater";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_Closing);
            this.Load += new System.EventHandler(this.Form_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button b_CheckUpdate;
        private System.Windows.Forms.Label l_CurrentVersion;
        private System.Windows.Forms.Label l_NewestVersion;
        private System.Windows.Forms.CheckBox cb_Startup;
        private System.Windows.Forms.Button b_Exit;
        private System.Windows.Forms.CheckBox cb_CheckUpateOnClick;
        private System.Windows.Forms.CheckBox cb_HideConfig;
        private System.Windows.Forms.Button b_DeleteConfig;
        private System.Windows.Forms.CheckBox cb_CheckSelfUpdate;
        private System.Windows.Forms.Button b_SelfUpdateCheck;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numList;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label l_Progress;
        private System.Windows.Forms.Button b_CancelDownload;
        private System.Windows.Forms.Label l_DownloadAmount;
        private System.Windows.Forms.Label l_DownloadSpeed;
        private System.Windows.Forms.CheckBox cb_EnableRegularChecks;
        private System.Windows.Forms.ComboBox comboBox_RegularUpdateInterval;
        private System.Windows.Forms.CheckBox cb_ShowUpToDateNotif;
        private System.Windows.Forms.Button b_OpenInstallFolder;
    }
}

