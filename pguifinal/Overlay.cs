using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

        public Overlay()
        {
            InitializeComponent();
        }

        public volatile bool m_StopThread;

        #region [ THREADS ]
        Thread thread1;
        Thread thread2;
        #endregion

        private void Overlay_Load(object sender, EventArgs e)
        {
            thread1 = new Thread(Pingloop);
            thread2 = new Thread(WindowCheck);

            thread1.IsBackground = true;
            thread1.Start();
            thread2.IsBackground = true;
            thread2.Start();
        }

        private void Pingloop()
        {
            while (!m_StopThread)
            {
                var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var ip = IPGlobalProperties.GetIPGlobalProperties();
                foreach (var tcp in ip.GetActiveTcpConnections())
                    if (tcp.RemoteEndPoint.Port == 10100)
                    {
                        string raddress = tcp.RemoteEndPoint.Address.ToString();
                        var times = new List<double>();
                        TcpClient cli = new TcpClient();
                        cli.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                        cli.Client.NoDelay = false;
                        var endpoint = new IPEndPoint(IPAddress.Any, 0);
                        cli.Client.Bind(endpoint);
                        var stopwatch = new Stopwatch();
                        stopwatch.Start(); // Start 
                        cli.Client.Connect(raddress, 10100);
                        stopwatch.Stop(); // Stop
                        cli.Client.Close();
                        double t = stopwatch.Elapsed.TotalMilliseconds;
                        times.Add(t);
                        this.Invoke((MethodInvoker)delegate
                        {
                            customLabel1.Text = string.Format("{0:0.00} MS", t);
                        });
                        break;
                    }
                Thread.Sleep(3000);
            }
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
                Thread.Sleep(4000);
            }
        }
    }
}
