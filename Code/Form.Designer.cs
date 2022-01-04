
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
            this.l_State = new System.Windows.Forms.Label();
            this.b_SelfUpdateCheck = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // b_CheckUpdate
            // 
            this.b_CheckUpdate.Location = new System.Drawing.Point(159, 120);
            this.b_CheckUpdate.Name = "b_CheckUpdate";
            this.b_CheckUpdate.Size = new System.Drawing.Size(190, 25);
            this.b_CheckUpdate.TabIndex = 6;
            this.b_CheckUpdate.Text = "Check for Chromium updates";
            this.b_CheckUpdate.UseVisualStyleBackColor = true;
            this.b_CheckUpdate.Click += new System.EventHandler(this.b_CheckUpdate_Click);
            // 
            // l_CurrentVersion
            // 
            this.l_CurrentVersion.AutoSize = true;
            this.l_CurrentVersion.Location = new System.Drawing.Point(250, 9);
            this.l_CurrentVersion.Name = "l_CurrentVersion";
            this.l_CurrentVersion.Size = new System.Drawing.Size(105, 17);
            this.l_CurrentVersion.TabIndex = 1;
            this.l_CurrentVersion.Text = "Current Version: ";
            // 
            // l_NewestVersion
            // 
            this.l_NewestVersion.AutoSize = true;
            this.l_NewestVersion.Location = new System.Drawing.Point(250, 36);
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
            this.b_Exit.Location = new System.Drawing.Point(355, 120);
            this.b_Exit.Name = "b_Exit";
            this.b_Exit.Size = new System.Drawing.Size(75, 25);
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
            this.b_DeleteConfig.Location = new System.Drawing.Point(335, 89);
            this.b_DeleteConfig.Name = "b_DeleteConfig";
            this.b_DeleteConfig.Size = new System.Drawing.Size(95, 25);
            this.b_DeleteConfig.TabIndex = 7;
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
            // l_State
            // 
            this.l_State.AutoSize = true;
            this.l_State.Location = new System.Drawing.Point(250, 62);
            this.l_State.Name = "l_State";
            this.l_State.Size = new System.Drawing.Size(37, 17);
            this.l_State.TabIndex = 7;
            this.l_State.Text = "State";
            // 
            // b_SelfUpdateCheck
            // 
            this.b_SelfUpdateCheck.Location = new System.Drawing.Point(12, 120);
            this.b_SelfUpdateCheck.Name = "b_SelfUpdateCheck";
            this.b_SelfUpdateCheck.Size = new System.Drawing.Size(141, 25);
            this.b_SelfUpdateCheck.TabIndex = 5;
            this.b_SelfUpdateCheck.Text = "Check for self update";
            this.b_SelfUpdateCheck.Click += new System.EventHandler(this.CheckSelfUpdateClicked);
            // 
            // Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(442, 157);
            this.Controls.Add(this.b_SelfUpdateCheck);
            this.Controls.Add(this.l_State);
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
        private System.Windows.Forms.Label l_State;
        private System.Windows.Forms.Button b_SelfUpdateCheck;
    }
}

