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
        static int[] intervals  = new int[] { 90000, 30000};

        static string process_name = "";
        static string server_launch_script = "";
        static int proc_id = 0;

        static void Main(string[] args)
        {
            Process[] processes = Process.GetProcesses();
            if(processes != null)
            {
                using (StreamReader r = new StreamReader("./config.json"))
                {
                    dynamic json = JsonConvert.DeserializeObject(r.ReadToEnd());
                    process_name = json["process_name"];
                    server_launch_script = json["server_launch"];
                }

                foreach (Process p in processes)
                {                    
                    if (p.MainWindowTitle.Contains(process_name))
                    {                           
                        proc_id = p.Id;
                        hWnd = p.MainWindowHandle;

                        SetForegroundWindow(hWnd);
                        System.Windows.Forms.SendKeys.SendWait("say Server will restart in two minutes!");
                        System.Windows.Forms.SendKeys.SendWait("{ENTER}");
                        System.Timers.Timer timer = new System.Timers.Timer();
                        timer.Elapsed += new ElapsedEventHandler(FirstRestartNotification);
                        timer.Interval = intervals[0];
                        timer.Enabled = true;
                        timer.AutoReset = false;
                        break;                                               
                    }
                }
                Console.ReadLine();
            }
            Environment.Exit(0);
        }

        private static void FirstRestartNotification(object source, ElapsedEventArgs e)
        {            
            System.Windows.Forms.SendKeys.SendWait("say Server will restart in 30 seconds!");
            System.Windows.Forms.SendKeys.SendWait("{ENTER}");
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += new ElapsedEventHandler(Restart);
            timer.Interval = intervals[1];
            timer.Enabled = true;
            timer.AutoReset = false;
        }

        private static void Restart(object source, ElapsedEventArgs e)
        {            
            System.Windows.Forms.SendKeys.SendWait("say Server stopping!");
            System.Windows.Forms.SendKeys.SendWait("{ENTER}");
            System.Windows.Forms.SendKeys.SendWait("stop");
            System.Windows.Forms.SendKeys.SendWait("{ENTER}");
            ExitServerConsole();       
        }

        private static void ExitServerConsole()
        {
            Process process = Process.GetProcessById(proc_id);
            process.Kill();

            string path = server_launch_script;
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.WorkingDirectory = Path.GetDirectoryName(path);
            psi.FileName = Path.GetFileName(path);
            Process.Start(psi);
            Environment.Exit(0);
        }
    }
}
