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

            // We check for an auto updater event here
            AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;
            label2.Text = ("You are using version " + Application.ProductVersion);
            button2.Enabled = false;
        }

        private Overlay Ov;

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

        private void CheckForClient()
        {
            Process[] processes = Process.GetProcessesByName("Client");

            foreach (Process p in processes)
            {
                if (p.MainWindowTitle == "Blade & Soul")
                {
                    label1.Text = (p.MainWindowTitle + " | " + p.Id + " | Running");
                    WindowState = FormWindowState.Minimized;
                    Ov = new Overlay();
                    Ov.Show();
                }
                else if (p.MainWindowTitle == "劍靈")
                {
                    label1.Text = (p.MainWindowTitle + " | " + p.Id + " | Running");
                    WindowState = FormWindowState.Minimized;
                    Ov = new Overlay();
                    Ov.Show();
                }
                else
                {
                    label1.Text = "Unable to detect client";
                    MessageBox.Show("Could not detect the client");
                }
            }
        }

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
    }
}
