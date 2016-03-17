using AutoUpdaterDotNET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pguifinal
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            // Check for update event
            AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;
            // Fill label with version information
            label2.Text = ("You are using version " + Application.ProductVersion);
            // Disable button 2
            button2.Enabled = false;
        }

        // Store form handle
        private Overlay Ov;

        #region [ Autoupdate Handler ]
        // Event handler for autoupdater
        private void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            if (args != null)
            {
                if (args.IsUpdateAvailable)
                {
                    var dialogResult = MessageBox.Show(string.Format(
                        "The version {0} available for download!"
                        + Environment.NewLine +
                        "Currently you are running on version {1}."
                        + Environment.NewLine +
                        "Would you like to update the application now?",
                        args.CurrentVersion, args.InstalledVersion), @"Update",
                        MessageBoxButtons.YesNo);

                    if (dialogResult.Equals(DialogResult.Yes))
                    {
                        try
                        {
                            AutoUpdater.DownloadUpdate();
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show(exception.Message, exception.GetType().ToString(), MessageBoxButtons.OK);
                        }
                    }
                }
                else
                {
                    MessageBox.Show(@"You are already using the latest version!", @"No update needed",
                    MessageBoxButtons.OK);
                }
            }
            else
            {
                MessageBox.Show(@"There was a problem reaching the update server", @"Update check failed",
                MessageBoxButtons.OK);
            }
        }
        #endregion

        #region [ Client Check ]
        // Check if client is running to prevent user errors
        private void CheckForClient()
        {
            Process[] processes = Process.GetProcessesByName("Client");

            foreach (Process p in processes)
            {
                // NCSOFT = Western / Japanese Version
                if (p.MainWindowTitle == "Blade & Soul")
                {
                    label1.Text = (p.MainWindowTitle + " | " + p.Id + " | " + p.MainWindowHandle + " | Running");
                    WindowState = FormWindowState.Minimized;
                    Ov = new Overlay();
                    Ov.Show();
                }
                // NCSOFT = Chinese / Taiwanese Version
                else if (p.MainWindowTitle == "劍靈")
                {
                    label1.Text = (p.MainWindowTitle + " | " + p.Id + " | " + p.MainWindowHandle + " | Running");
                    WindowState = FormWindowState.Minimized;
                    Ov = new Overlay();
                    Ov.Show();
                }
                else
                {
                    // Throw an error if we did not find the client
                    label1.Text = "Unable to detect client";
                    MessageBox.Show("Could not detect the client");
                }
            }
        }
        #endregion

        #region [ Buttons ]
        private void button1_Click(object sender, EventArgs e)
        {
            CheckForClient();
            button1.Enabled = false;
            button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Ov.m_StopThread = true;
            Ov.Close();
            label1.Text = "Stopped!";
            button1.Enabled = true;
            button2.Enabled = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AutoUpdater.Start("https://dvmiyo.com/software/updates/pgui/pupdate.xml");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.dvmiyo.com");
        }
        #endregion
    }
}
