using AutoUpdaterDotNET;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;
using Tesseract;
using System.Text;
using System.Timers;

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
            // Disable buttons
            button2.Enabled = false;
            button5.Enabled = false;
            button1.Enabled = false;

        }

        public class Strings
        {
            // Get Blade and Soul directory from registry
            public static string GamePathWestern = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\NCWest\BnS", "BaseDir", null);
            public static string GamePathTaiwan = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\NCTaiwan\TWBNS22", "BaseDir", null);
            public static string GamePathChina = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Tencent\BNS", "InstallPath", null);
        }

        // OCR - Damage Meter - Screenshot
        private void Screenshot()
        {
            Rectangle rect = new Rectangle(0, 820, 500, 200);
            Bitmap bmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(rect.Left, rect.Top, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);
            bmp.Save(@"test.tif", System.Drawing.Imaging.ImageFormat.Tiff);
        }

        private void CreateLogWest()
        {
            string GameLogsWestern = Strings.GamePathWestern + @"\bin\extraMode.ini";
            string LogVariables = "[Mode]" + Environment.NewLine + "DuelSkillLog=1";
            if (Directory.Exists(Strings.GamePathWestern))
            {
                if (File.Exists(GameLogsWestern))
                {
                    textBox1.AppendText("[DEBUG-EUNA]: File already exist!");
                    textBox1.AppendText("\r\n[DEBUG-EUNA]: Enter server then press enable!\r\n");
                    button1.Enabled = true;
                }
                else
                {
                    File.WriteAllText(GameLogsWestern, LogVariables);
                    textBox1.AppendText("\r\n[DEBUG-EUNA]: Created file");
                    textBox1.AppendText("\r\n[DEBUG-EUNA]: " + GameLogsWestern.ToString());
                    textBox1.AppendText("\r\n[DEBUG-EUNA]: Enter server then press enable!\r\n");
                    button1.Enabled = true;
                }
            }

        }

        private void CreateLogChina()
        {
            string GameLogsChina = Strings.GamePathChina + @"\bin\extraMode.ini";
            string LogVariables = "[Mode]" + Environment.NewLine + "DuelSkillLog=1";
            if (Directory.Exists(Strings.GamePathChina))
            {
                if (File.Exists(GameLogsChina))
                {
                    textBox1.AppendText("\r\n[DEBUG-CN]: File already exist!");
                    textBox1.AppendText("\r\n[DEBUG-CN]: Enter server then press enable!\r\n");
                    button1.Enabled = true;
                }
                else
                {
                    File.WriteAllText(GameLogsChina, LogVariables);
                    textBox1.AppendText("\r\n[DEBUG-CN]: Created file");
                    textBox1.AppendText("\r\n[DEBUG-CN]: " + GameLogsChina.ToString());
                    textBox1.AppendText("\r\n[DEBUG-CN]: Enter server then press enable!\r\n");
                    button1.Enabled = true;
                }
            }

        }

        private void CreateLogTaiwan()
        {
            string GameLogsTaiwan = Strings.GamePathTaiwan + @"bin\extraMode.ini";
            string LogVariables = "[Mode]" + Environment.NewLine + "DuelSkillLog=1";
            if (Directory.Exists(Strings.GamePathTaiwan))
            {
                if (File.Exists(GameLogsTaiwan))
                {
                    textBox1.AppendText("\r\n[DEBUG-TW]: File already exist!");
                    textBox1.AppendText("\r\n[DEBUG-TW]: Enter server then press enable!\r\n");
                    button1.Enabled = true;
                }
                else
                {
                    File.WriteAllText(GameLogsTaiwan, LogVariables);
                    textBox1.AppendText("\r\n[DEBUG-TW]: Created file");
                    textBox1.AppendText("\r\n[DEBUG-TW]: " + GameLogsTaiwan.ToString());
                    textBox1.AppendText("\r\n[DEBUG-TW]: Enter server then press enable!\r\n");
                    button1.Enabled = true;
                }
            }


        }

        // Store form handle
        public Overlay Ov;

        public string SelectedComboValue
        {
            get { return comboBox1.SelectedItem.ToString(); }
        }

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
        public void CheckForClient()
        {

            Process[] processes = Process.GetProcessesByName("Client");

            foreach (Process p in processes)
            {
                // NCSOFT = Western / Japanese Version
                if (p.MainWindowTitle == "Blade & Soul")
                {
                    label1.Text = (p.MainWindowTitle + " | " + p.Id + " | " + p.MainWindowHandle + " | Running");
                    WindowState = FormWindowState.Minimized;
                    if (Ov == null)
                        Ov = new Overlay(this);
                        Ov.Owner = this;
                        Ov.Show();
                }
                // NCSOFT = Chinese / Taiwanese Version
                else if (p.MainWindowTitle == "劍靈")
                {
                    label1.Text = (p.MainWindowTitle + " | " + p.Id + " | " + p.MainWindowHandle + " | Running");
                    WindowState = FormWindowState.Minimized;
                    if (Ov == null)
                        Ov = new Overlay(this);
                    Ov.Owner = this;
                    Ov.Show();
                }
                else
                {
                    // Throw an error if we did not find the client
                    label1.Text = "Unable to detect client";
                    // MessageBox.Show("Could not detect the client");
                }
            }
        }
        #endregion

        #region [ Buttons ]
        private void button1_Click(object sender, EventArgs e)
        {
            { 
                CheckForClient();
                button1.Enabled = false;
                button2.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Ov.m_StopThread = true;
            Ov.aTimer.Close();
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

        private void button5_Click(object sender, EventArgs w)
        {
            Screenshot(); // Create a screenshot of the damage log
            var CombatLog = "test.tif"; // Damage log screenshot
            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
            {
                using (var img = Pix.LoadFromFile(CombatLog))
                {
                    using (var page = engine.Process(img))
                    {
                        var text = page.GetText();
                        Console.WriteLine("Mean confidence: {0}", page.GetMeanConfidence());

                        textBox2.AppendText(text.ToString());

                        using (var iter = page.GetIterator())
                        {
                            iter.Begin();

                            do
                            {
                                do
                                {
                                    do
                                    {
                                        do
                                        {
                                            if (iter.IsAtBeginningOf(PageIteratorLevel.Block))
                                            {
                                                Console.WriteLine("<BLOCK>");
                                            }

                                            Console.Write(iter.GetText(PageIteratorLevel.Word));
                                            Console.Write(" ");

                                            if (iter.IsAtFinalOf(PageIteratorLevel.TextLine, PageIteratorLevel.Word))
                                            {
                                                Console.WriteLine();
                                            }
                                        } while (iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word));

                                        if (iter.IsAtFinalOf(PageIteratorLevel.Para, PageIteratorLevel.TextLine))
                                        {
                                            Console.WriteLine();
                                        }
                                    } while (iter.Next(PageIteratorLevel.Para, PageIteratorLevel.TextLine));
                                } while (iter.Next(PageIteratorLevel.Block, PageIteratorLevel.Para));
                            } while (iter.Next(PageIteratorLevel.Block));
                        }
                    }
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "North America")
            {
                CreateLogWest();
            }
            else if (comboBox1.Text == "Europe")
            {
                CreateLogWest();
            }
            else if (comboBox1.Text == "Taiwan")
            {
                CreateLogTaiwan();
            }
            else if (comboBox1.Text == "China")
            {
                CreateLogChina();
            }
        }
        #endregion
    }
}
