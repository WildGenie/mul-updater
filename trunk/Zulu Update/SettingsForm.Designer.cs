namespace Zulu_Update
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tboxMULsDir = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.dlgBrowseDir = new System.Windows.Forms.FolderBrowserDialog();
            this.rbRunClient = new System.Windows.Forms.RadioButton();
            this.rbRunNone = new System.Windows.Forms.RadioButton();
            this.rbRunInjection = new System.Windows.Forms.RadioButton();
            this.cboxAutoClose = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblCopyrght = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tboxMULsDir
            // 
            this.tboxMULsDir.Location = new System.Drawing.Point(151, 61);
            this.tboxMULsDir.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tboxMULsDir.Name = "tboxMULsDir";
            this.tboxMULsDir.Size = new System.Drawing.Size(190, 26);
            this.tboxMULsDir.TabIndex = 0;
            this.tboxMULsDir.TextChanged += new System.EventHandler(this.tboxMULsDir_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Ultima Online dir:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(348, 60);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(84, 29);
            this.button1.TabIndex = 2;
            this.button1.Text = "Browse ...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // dlgBrowseDir
            // 
            this.dlgBrowseDir.ShowNewFolderButton = false;
            this.dlgBrowseDir.HelpRequest += new System.EventHandler(this.dlgBrowseDir_HelpRequest);
            // 
            // rbRunClient
            // 
            this.rbRunClient.AutoSize = true;
            this.rbRunClient.Location = new System.Drawing.Point(105, 26);
            this.rbRunClient.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbRunClient.Name = "rbRunClient";
            this.rbRunClient.Size = new System.Drawing.Size(103, 24);
            this.rbRunClient.TabIndex = 3;
            this.rbRunClient.TabStop = true;
            this.rbRunClient.Text = "Client.exe";
            this.rbRunClient.UseVisualStyleBackColor = true;
            this.rbRunClient.CheckedChanged += new System.EventHandler(this.rbRunClient_CheckedChanged);
            // 
            // rbRunNone
            // 
            this.rbRunNone.AutoSize = true;
            this.rbRunNone.Location = new System.Drawing.Point(213, 26);
            this.rbRunNone.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbRunNone.Name = "rbRunNone";
            this.rbRunNone.Size = new System.Drawing.Size(72, 24);
            this.rbRunNone.TabIndex = 4;
            this.rbRunNone.TabStop = true;
            this.rbRunNone.Text = "None";
            this.rbRunNone.UseVisualStyleBackColor = true;
            this.rbRunNone.CheckedChanged += new System.EventHandler(this.rbRunNone_CheckedChanged);
            // 
            // rbRunInjection
            // 
            this.rbRunInjection.AutoSize = true;
            this.rbRunInjection.Location = new System.Drawing.Point(7, 26);
            this.rbRunInjection.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbRunInjection.Name = "rbRunInjection";
            this.rbRunInjection.Size = new System.Drawing.Size(94, 24);
            this.rbRunInjection.TabIndex = 5;
            this.rbRunInjection.TabStop = true;
            this.rbRunInjection.Text = "Injection";
            this.rbRunInjection.UseVisualStyleBackColor = true;
            this.rbRunInjection.CheckedChanged += new System.EventHandler(this.rbRunInjection_CheckedChanged);
            // 
            // cboxAutoClose
            // 
            this.cboxAutoClose.AutoSize = true;
            this.cboxAutoClose.Location = new System.Drawing.Point(17, 195);
            this.cboxAutoClose.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cboxAutoClose.Name = "cboxAutoClose";
            this.cboxAutoClose.Size = new System.Drawing.Size(214, 24);
            this.cboxAutoClose.TabIndex = 6;
            this.cboxAutoClose.Text = "Close Update Application";
            this.cboxAutoClose.UseVisualStyleBackColor = true;
            this.cboxAutoClose.CheckedChanged += new System.EventHandler(this.chckBoxAutoClose_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbRunInjection);
            this.groupBox1.Controls.Add(this.rbRunClient);
            this.groupBox1.Controls.Add(this.rbRunNone);
            this.groupBox1.Location = new System.Drawing.Point(17, 108);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(290, 68);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Run when finished";
            // 
            // lblCopyrght
            // 
            this.lblCopyrght.AutoSize = true;
            this.lblCopyrght.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblCopyrght.Location = new System.Drawing.Point(198, 289);
            this.lblCopyrght.Name = "lblCopyrght";
            this.lblCopyrght.Size = new System.Drawing.Size(233, 20);
            this.lblCopyrght.TabIndex = 8;
            this.lblCopyrght.Text = "Mul Updater by AnDenixa, 2014";
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(447, 321);
            this.Controls.Add(this.lblCopyrght);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cboxAutoClose);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tboxMULsDir);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "SettingsForm";
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tboxMULsDir;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.FolderBrowserDialog dlgBrowseDir;
        private System.Windows.Forms.RadioButton rbRunClient;
        private System.Windows.Forms.RadioButton rbRunNone;
        private System.Windows.Forms.RadioButton rbRunInjection;
        private System.Windows.Forms.CheckBox cboxAutoClose;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblCopyrght;
    }
}