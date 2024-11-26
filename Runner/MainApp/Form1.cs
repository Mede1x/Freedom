using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainApp
{
    public partial class Form1 : Form
    {


        static bool isHookActive = false;


        public Form1()
        {
            InitializeComponent();
        }


        static void MonitorProcess(string processName)
        {
            while (true)
            {
                bool processExists = Process.GetProcessesByName(processName).Length > 0;

                if (processExists && !isHookActive)
                {
                    KeyboardInterceptor.StartHook();
                    isHookActive = true;
                    Console.WriteLine($"{processName} detected. Keyboard hook started.");
                }
                else if (!processExists && isHookActive)
                {
                    KeyboardInterceptor.StopHook();
                    isHookActive = false;
                    Console.WriteLine($"{processName} not detected. Keyboard hook stopped.");
                }

                Thread.Sleep(1000); // Check every second
            }
        }
    


        void ex()
        {
            MonitorProcess("app.exe");
        }

        void x(string pa)
        {
            Process.Start(pa);
        }


        private void Form1_Load(object sender, EventArgs e)
        {


            ex();

            string current_path = System.Environment.CurrentDirectory;

            string apps_1 = "HIDNV.exe";
            string apps_2 = "app.exe";

            x(apps_1);
            x(apps_2);

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            KeyboardInterceptor.StopHook();
        }
    }
}
