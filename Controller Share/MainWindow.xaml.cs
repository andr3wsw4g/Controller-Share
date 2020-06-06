using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Threading;
using System.Net.Sockets;
using vJoyInterfaceWrap;
using System.Data.SqlTypes;

namespace Controller_Share
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        List<ControllerHost> host = new List<ControllerHost>();
        int i;
        DispatcherTimer timer1;
        TextOutput console;
        public MainWindow()
        {
            i = 0;
            InitializeComponent();
            // Create the startup window
            console = new TextOutput(tConsole);
            Console.SetOut(console);
            Console.WriteLine("Started");
            //timer1 = new DispatcherTimer();
            //timer1.Interval = TimeSpan.FromMilliseconds(1000);
            //timer1.Tick += timer_Tick;
            //timer1.Start();
            
        }
        
        void timer_Tick(object sender, EventArgs e)
        {
            
            Console.WriteLine(i);
            i++;
            
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            ControllerConnection client = new ControllerConnection("192.168.0.23",25565, System.Net.Sockets.SocketType.Stream);
        }

        private void btnHost_Click(object sender, RoutedEventArgs e)
        {
            if (host.Count > 0)
            {
                Console.WriteLine("Program is already hosting the server.");
                return;
            }
            host.Add( new ControllerHost());
            var t = Task.Run(()=> { host[0].Listen(25565, SocketType.Stream); });

        }
    }
}
