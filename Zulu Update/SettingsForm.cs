// Mul Updater
// Settings Form
// 
// by AnDenix

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace Zulu_Update
{
    public partial class SettingsForm : Form
    {

        private MainForm parentForm;
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var res = dlgBrowseDir.ShowDialog();
            if (res == DialogResult.OK)
            {
                tboxMULsDir.Text = dlgBrowseDir.SelectedPath;              
                parentForm.setCfgValue("ultima_dir", dlgBrowseDir.SelectedPath);
            }
        }

        private void dlgBrowseDir_HelpRequest(object sender, EventArgs e)
        {

        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            parentForm = (MainForm)this.Owner;
            tboxMULsDir.Text = parentForm.getCfgValue("ultima_dir");
            switch (parentForm.runWhenFinished)
            {
                case MainForm.RunApplications.None:
                    rbRunNone.Select();
                    break;
                case MainForm.RunApplications.Client:
                    rbRunClient.Select();
                    break;
                case MainForm.RunApplications.Injection:
                    rbRunInjection.Select();
                    break;
            }

            if (parentForm.getCfgValueInt("closeWhenFinished") == 1)
                cboxAutoClose.CheckState = CheckState.Checked;
            else
                cboxAutoClose.CheckState = CheckState.Unchecked;
        }

        private void tboxMULsDir_TextChanged(object sender, EventArgs e)
        {
            parentForm.updateCfgValue("ultima_dir", ((TextBox)sender).Text);
        }

        private void rbRunInjection_CheckedChanged(object sender, EventArgs e)
        {
            parentForm.runWhenFinished = MainForm.RunApplications.Injection;
            parentForm.setCfgValue("runWhenFinished", (int)parentForm.runWhenFinished);
        }

        private void rbRunClient_CheckedChanged(object sender, EventArgs e)
        {
            parentForm.runWhenFinished = MainForm.RunApplications.Client;
            parentForm.setCfgValue("runWhenFinished", (int)parentForm.runWhenFinished);
        }

        private void rbRunNone_CheckedChanged(object sender, EventArgs e)
        {
            parentForm.runWhenFinished = MainForm.RunApplications.None;
            parentForm.setCfgValue("runWhenFinished", (int)parentForm.runWhenFinished);
        }

        private void chckBoxAutoClose_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
                parentForm.setCfgValue("closeWhenFinished", 1);
            else
                parentForm.setCfgValue("closeWhenFinished", 0);
        }
    }
}
