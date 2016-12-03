using Server.Util;
using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Server
{

    public partial class MainWindow : Window
    {

        public static MainWindow instance;

        public MainWindow()
        {
            instance = this;
            InitializeComponent();

            DBContext.getInstace();
            try
            {
                ServerMethods.IntiServer();
                Log("Server Sucessfully Started");

                new Thread(() =>
                {
                    ServerMethods.BeginListening();
                }).Start();
            }
            catch (Exception)
            {
                Log("Server Failed To Start");
            }
        }

        public void Log(string logMessage)
        {
            Application.Current.Dispatcher.Invoke((Action)(() =>{ logTextBox.Text += logMessage+
                "\n----------------------------------------------------------------------------------------------\n";}));
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            ServerMethods.tcpListener.Stop();
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (ClientsTab.IsSelected)
            {
                ClientsDataGrid.ItemsSource = DBContext.getInstace().Clients.ToArray();
            }

            if (TransactionsTab.IsSelected)
            {
                TransactionsDataGrid.ItemsSource = DBContext.getInstace().Transactions.ToArray();
            }
        }
    }
}
