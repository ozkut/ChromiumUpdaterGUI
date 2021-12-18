
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
            this.SuspendLayout();
            // 
            // b_CheckUpdate
            // 
            this.b_CheckUpdate.Location = new System.Drawing.Point(12, 66);
            this.b_CheckUpdate.Name = "b_CheckUpdate";
            this.b_CheckUpdate.Size = new System.Drawing.Size(124, 27);
            this.b_CheckUpdate.TabIndex = 0;
            this.b_CheckUpdate.Text = "Check for updates";
            this.b_CheckUpdate.UseVisualStyleBackColor = true;
            this.b_CheckUpdate.Click += new System.EventHandler(this.b_CheckUpdate_Click);
            // 
            // l_CurrentVersion
            // 
            this.l_CurrentVersion.AutoSize = true;
            this.l_CurrentVersion.Location = new System.Drawing.Point(12, 9);
            this.l_CurrentVersion.Name = "l_CurrentVersion";
            this.l_CurrentVersion.Size = new System.Drawing.Size(105, 17);
            this.l_CurrentVersion.TabIndex = 1;
            this.l_CurrentVersion.Text = "Current Version: ";
            // 
            // l_NewestVersion
            // 
            this.l_NewestVersion.AutoSize = true;
            this.l_NewestVersion.Location = new System.Drawing.Point(12, 36);
            this.l_NewestVersion.Name = "l_NewestVersion";
            this.l_NewestVersion.Size = new System.Drawing.Size(105, 17);
            this.l_NewestVersion.TabIndex = 2;
            this.l_NewestVersion.Text = "Newest Version: ";
            // 
            // cb_Startup
            // 
            this.cb_Startup.AutoSize = true;
            this.cb_Startup.Location = new System.Drawing.Point(220, 39);
            this.cb_Startup.Name = "cb_Startup";
            this.cb_Startup.Size = new System.Drawing.Size(105, 21);
            this.cb_Startup.TabIndex = 4;
            this.cb_Startup.Text = "Start on boot";
            this.cb_Startup.UseVisualStyleBackColor = true;
            this.cb_Startup.CheckedChanged += new System.EventHandler(this.cb_Startup_CheckedChanged);
            // 
            // b_Exit
            // 
            this.b_Exit.Location = new System.Drawing.Point(355, 70);
            this.b_Exit.Name = "b_Exit";
            this.b_Exit.Size = new System.Drawing.Size(75, 23);
            this.b_Exit.TabIndex = 5;
            this.b_Exit.Text = "Exit";
            this.b_Exit.UseVisualStyleBackColor = true;
            this.b_Exit.Click += new System.EventHandler(this.b_Exit_Click);
            // 
            // cb_CheckUpateOnClick
            // 
            this.cb_CheckUpateOnClick.AutoSize = true;
            this.cb_CheckUpateOnClick.Checked = true;
            this.cb_CheckUpateOnClick.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_CheckUpateOnClick.Location = new System.Drawing.Point(220, 12);
            this.cb_CheckUpateOnClick.Name = "cb_CheckUpateOnClick";
            this.cb_CheckUpateOnClick.Size = new System.Drawing.Size(210, 21);
            this.cb_CheckUpateOnClick.TabIndex = 6;
            this.cb_CheckUpateOnClick.Text = "Check for updates on maximise";
            this.cb_CheckUpateOnClick.UseVisualStyleBackColor = true;
            this.cb_CheckUpateOnClick.CheckedChanged += new System.EventHandler(this.cb_CheckUpateOnClick_CheckedChanged);
            // 
            // cb_HideConfig
            // 
            this.cb_HideConfig.AutoSize = true;
            this.cb_HideConfig.Checked = true;
            this.cb_HideConfig.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_HideConfig.Location = new System.Drawing.Point(220, 66);
            this.cb_HideConfig.Name = "cb_HideConfig";
            this.cb_HideConfig.Size = new System.Drawing.Size(115, 21);
            this.cb_HideConfig.TabIndex = 7;
            this.cb_HideConfig.Text = "Hide config file";
            this.cb_HideConfig.UseVisualStyleBackColor = true;
            this.cb_HideConfig.CheckedChanged += new System.EventHandler(this.cb_HideConfig_CheckedChanged);
            // 
            // b_DeleteConfig
            // 
            this.b_DeleteConfig.Location = new System.Drawing.Point(331, 39);
            this.b_DeleteConfig.Name = "b_DeleteConfig";
            this.b_DeleteConfig.Size = new System.Drawing.Size(99, 27);
            this.b_DeleteConfig.TabIndex = 8;
            this.b_DeleteConfig.Text = "Delete Config";
            this.b_DeleteConfig.UseVisualStyleBackColor = true;
            this.b_DeleteConfig.Click += new System.EventHandler(this.b_DeleteConfig_Click);
            // 
            // Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(442, 101);
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
        private System.Windows.Forms.Label l_ProgramStatus;
        private System.Windows.Forms.CheckBox cb_Startup;
        private System.Windows.Forms.Button b_Exit;
        private System.Windows.Forms.CheckBox cb_CheckUpateOnClick;
        private System.Windows.Forms.CheckBox cb_HideConfig;
        private System.Windows.Forms.Button b_DeleteConfig;
    }
}

