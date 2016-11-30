using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string lableContent;

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                server.IntiServer();
                clientAccount.fillList();
                serverStarted.Content = "server sucessfully started";


                try
                {
                    var t1 = new Thread(listenToserver);
                    t1.Start(this);

                }
                catch (Exception)
                {

                    displayStatus("connection error");
                }

            }
            catch (Exception e)
            {
                serverStarted.Content = "server failed to start";
            }


        }

        public void displayStatus(string s)
        {
            lableContent = s;
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action<Label>(setlableValue), connectionstatus);
        }

        public static void listenToserver(object arg)
        {
            MainWindow mainWindow = (MainWindow)arg;
            server.Listening(mainWindow);
        }

        private static void setlableValue(Label l)
        {

            l.Content += lableContent;
        }

        private void CreatUserButton_OnClick(object sender, RoutedEventArgs e)
        {
            var windows = new CreatUserWindows();
            windows.Activate();
            windows.InitializeComponent();
            windows.Show();
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            server.tcpListener.Stop();
        }
    }

}
