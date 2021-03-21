using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using System.Timers;
using System.IO;
using Newtonsoft.Json;

namespace SendKeys
{    
    class Program
    {
        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr p);
        static IntPtr hWnd = (IntPtr)0;

        static void Main(string[] args)
        {
            Process[] processes = Process.GetProcesses();
            if(processes != null)
            {
                foreach(Process p in processes)
                {                    
                    if (p.MainWindowTitle.Contains("Minecraft"))
                    {
                        hWnd = p.MainWindowHandle;
                        SetForegroundWindow(hWnd);
                        System.Windows.Forms.SendKeys.SendWait("say Server will restart in two minutes!");
                        System.Windows.Forms.SendKeys.SendWait("{ENTER}");
                        System.Timers.Timer timer = new System.Timers.Timer();
                        timer.Elapsed += new ElapsedEventHandler(FirstRestartNotification);
                        timer.Interval = 90000;
                        timer.Enabled = true;
                        timer.AutoReset = false;                        
                    }
                }
                Console.ReadLine();
            }            
        }

        private static void FirstRestartNotification(object source, ElapsedEventArgs e)
        {
            SetForegroundWindow(hWnd);
            System.Windows.Forms.SendKeys.SendWait("say Server will restart in 30 seconds!");
            System.Windows.Forms.SendKeys.SendWait("{ENTER}");
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += new ElapsedEventHandler(Restart);
            timer.Interval = 30000;
            timer.Enabled = true;
            timer.AutoReset = false;
        }

        private static void Restart(object source, ElapsedEventArgs e)
        {
            SetForegroundWindow(hWnd);
            System.Windows.Forms.SendKeys.SendWait("say Server stopping!");
            System.Windows.Forms.SendKeys.SendWait("{ENTER}");
            System.Windows.Forms.SendKeys.SendWait("stop");
            System.Windows.Forms.SendKeys.SendWait("{ENTER}");
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += new ElapsedEventHandler(ExitServerConsole);
            timer.Interval = 6000;
            timer.Enabled = true;
            timer.AutoReset = false;            
        }

        private static void ExitServerConsole(object source, ElapsedEventArgs e)
        {
            string path = @"C:\Users\Administrator\Desktop\Minecraft Servers\the-1122-pack_1.3.2\LaunchServer.bat";
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.WorkingDirectory = Path.GetDirectoryName(path);
            psi.FileName = Path.GetFileName(path);
            Process.Start(psi);

            SetForegroundWindow(hWnd);            
            System.Windows.Forms.SendKeys.SendWait("{ENTER}");
            System.Windows.Forms.SendKeys.SendWait("{ENTER}");            
            Environment.Exit(0);
        }
    }
}
