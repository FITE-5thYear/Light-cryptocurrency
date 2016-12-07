using Server.Algorithms;
using Server.Util;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Server
{
    public partial class MainWindow : Window
    {
        private static string IP = "127.0.0.1";
        private static int HOST = 13000;
        private ServerObject server;

        public MainWindow()
        {
            instance = this;
            InitializeComponent();
            DBContext.getInstace();
            createServer();
        }

        private void createServer()
        {

            //generate public key for server
            KeysManager.generateAESPublicKey();
            KeysManager.generateRSAPublicKey(RSAAlgorithm.RSAServiceProvider);

            server = new ServerObject(IP, HOST);

            server.logger = Log;
            server.intiServer();

            server.startServer();

            server.respose = (s) => {

                string requstType = s.ReadString();
                ResposesManager.ProcessRequst(requstType,s);

            };
            
        }

        public void Log(string logMessage)
        {
            Application.Current.Dispatcher.Invoke((Action)(() =>{ logTextBox.Text += logMessage+
                "\n----------------------------------------------------------------------------------------------\n";}));
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            server.stopServer();
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

        public static MainWindow instance;
    }
}
