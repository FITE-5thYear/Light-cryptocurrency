using Server.Util;
using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Server
{

    public partial class ClientWindows : Window
    {
        private CryptocurrencyContext db;

        public ClientWindows()
        {
            InitializeComponent();

            db = new CryptocurrencyContext();
            try
            {
                ServerMethods.IntiServer();
                Log("Server Sucessfully Started.\n");
                new Thread(listenToserver).Start(this);
            }
            catch (Exception)
            {
                Log("Server Failed To Start.\n");
            }
        }

        public void Log(string logMessage)
        {
            Application.Current.Dispatcher.Invoke((Action)(() =>{ logTextBox.Text += logMessage;}));
        }

        public static void listenToserver(object arg)
        {
            ClientWindows mainWindow = (ClientWindows)arg;
            ServerMethods.BeginListening(mainWindow);
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            ServerMethods.tcpListener.Stop();
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (ClientsTab.IsSelected)
            {
               
                ClientsDataGrid.ItemsSource = db.Clients.ToArray();
            }

            if (TransactionsTab.IsSelected)
            {
                TransactionsDataGrid.ItemsSource = db.Transactions.ToArray();
            }
        }
    }
}
