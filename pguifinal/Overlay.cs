using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace pguifinal
{
    public partial class Overlay : Form
    {
        #region [ PINVOKES ]

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
        #endregion

        #region [ GET ACTIVE WINDOW TITLE ]
        private string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }
        #endregion

        #region [ ALT TAB HIDING ]       
        protected override CreateParams CreateParams
        {
            get
            {
                var Params = base.CreateParams;
                Params.ExStyle |= 0x80;
                return Params;
            }
        }
        #endregion

        Main _parentForm = null;

        public Overlay(Main parentForm1)
        {
            InitializeComponent();
            _parentForm = parentForm1;
        }

        public volatile bool m_StopThread;

        #region [ THREADS ]
        Thread thread2;
        #endregion

        private void Overlay_Load(object sender, EventArgs e)
        {
            thread2 = new Thread(WindowCheck);
            thread2.IsBackground = true;
            thread2.Start();
            runTimer();
        }

        public class Strings
        {
            // Get Blade and Soul directory from registry
            public static string GamePathWestern = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\NCWest\BnS", "BaseDir", null);
            public static string GamePathTaiwan = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\NCTaiwan\TWBNS22", "BaseDir", null);
            public static string GamePathChina = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Tencent\BNS", "InstallPath", null);
        }

        public static FileInfo GetNewestFile(DirectoryInfo directory)
        {
            // Get the latest log file
            return directory.GetFiles()
                .Union(directory.GetDirectories().Select(d => GetNewestFile(d)))
                .OrderByDescending(f => (f == null ? DateTime.MinValue : f.LastWriteTime))
                .FirstOrDefault();
        }

        // Store form handle
        public System.Timers.Timer aTimer = new System.Timers.Timer(10000);

        public void runTimer()
        {
            aTimer.Elapsed += new ElapsedEventHandler(RunEvent);
            aTimer.Interval = 3000;
            aTimer.Enabled = true;
        }

        // This method will get called every second until the timer stops or the program exits.
        public void RunEvent(object source, ElapsedEventArgs e)
        {
            string ChineseTitle = "劍靈";
            byte[] bytes = Encoding.Default.GetBytes(ChineseTitle);
            ChineseTitle = Encoding.UTF8.GetString(bytes);

            if (this.InvokeRequired)
                Invoke(new MethodInvoker(delegate ()
            {
                if (_parentForm.comboBox1.Text == "North America")
                {
                    if (GetActiveWindowTitle() == "Blade & Soul")
                    {
                        FileInfo newestFile = GetNewestFile(new DirectoryInfo(Strings.GamePathWestern + @"\log\"));
                        string GameLogs = Strings.GamePathWestern + @"\log\" + newestFile;
                        var lastLine = File.ReadLines(GameLogs).Last();
                        // Invoke((MethodInvoker)delegate
                        // {
                        var patternworld = @"(ping latency| min| max| avg) :( \d+)|[\S\s]";
                        var replacedworld = Regex.Replace(lastLine, patternworld, "$1$2");
                        customLabel1.Text = "LATENCY: " + replacedworld.ToUpper();
                        // });
                    }
                }
             

            else if (_parentForm.SelectedComboValue == "Europe")
            {
                if (GetActiveWindowTitle() == "Blade & Soul")
                {
                    FileInfo newestFile = GetNewestFile(new DirectoryInfo(Strings.GamePathWestern + @"\log\"));
                    string GameLogs = Strings.GamePathWestern + @"\log\" + newestFile;
                    var lastLine = File.ReadLines(GameLogs).Last();
                    Invoke((MethodInvoker)delegate
                    {
                        var patternworld = @"(ping latency| min| max| avg) :( \d+)|[\S\s]";
                        var replacedworld = Regex.Replace(lastLine, patternworld, "$1$2");
                        customLabel1.Text = "LATENCY: " + replacedworld.ToUpper();
                    });
                }
            }

            else if (_parentForm.comboBox1.Text == "Taiwan")
            {
                if (GetActiveWindowTitle() == ChineseTitle)
                {
                    FileInfo newestFile = GetNewestFile(new DirectoryInfo(Strings.GamePathTaiwan + @"\log\"));
                    string GameLogs = Strings.GamePathTaiwan + @"\log\" + newestFile;
                    var lastLine = File.ReadLines(GameLogs).Last();
                    Invoke((MethodInvoker)delegate
                    {
                        var patternworld = @"(ping latency| min| max| avg) :( \d+)|[\S\s]";
                        var replacedworld = Regex.Replace(lastLine, patternworld, "$1$2");
                        customLabel1.Text = "LATENCY: " + replacedworld.ToUpper();
                    });
                }
            }

            else if (_parentForm.comboBox1.Text == "China")
            {
                if (GetActiveWindowTitle() == ChineseTitle)
                {
                    FileInfo newestFile = GetNewestFile(new DirectoryInfo(Strings.GamePathChina + @"\log\"));
                    string GameLogs = Strings.GamePathChina + @"\log\" + newestFile;
                    var lastLine = File.ReadLines(GameLogs).Last();
                    Invoke((MethodInvoker)delegate
                    {
                        var patternworld = @"(ping latency| min| max| avg) :( \d+)|[\S\s]";
                        var replacedworld = Regex.Replace(lastLine, patternworld, "$1$2");
                        customLabel1.Text = "LATENCY: " + replacedworld.ToUpper();
                    });
                }
            }
            }));
        }

        private void WindowCheck()
        {
            string ChineseTitle = "劍靈";
            byte[] bytes = Encoding.Default.GetBytes(ChineseTitle);
            ChineseTitle = Encoding.UTF8.GetString(bytes);

            while (!m_StopThread)
            {
                if (GetActiveWindowTitle() == "Blade & Soul")
                {
                    Invoke((MethodInvoker)delegate
                    {
                        TopMost = true;
                        Show();
                    });
                }
                else if (GetActiveWindowTitle() == ChineseTitle)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        TopMost = true;
                        Show();
                    });
                }
                else
                {
                    Invoke((MethodInvoker)delegate
                    {
                        TopMost = false;
                        Hide();
                    });
                }
                Thread.Sleep(6000);
            }
        }
    }
}
