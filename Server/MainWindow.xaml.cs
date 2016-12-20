using Server.Algorithms;
using Server.Util;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Server
{
    public partial class MainWindow : Window
    {
        public static string IP = "127.0.0.1";
        public static int HOST = 13000;
        public static int CA = 12000;
        private ServerObject server;
        public  static ClientObject clientForCertificate;

        public MainWindow()
        {
            instance = this;
            InitializeComponent();
            DBContext.getInstace();
            clientForCertificate = new ClientObject();
            clientForCertificate.initClient(IP, CA);
            createServer();
        }

        private void createServer()
        {
            AES.getInstance();
            RSA rsa = new RSA("Server");

            KeysManager.generateAESKey();
            KeysManager.generateRSAPublicKey(rsa.rsaSP);
            KeysManager.generateRSAPrivateKey(rsa.rsaSP);
            
            server = new ServerObject(IP, HOST);

            server.logger = Log;
            server.intiServer();

            server.startServer();
            Log("Server Generated AES Public Key", Convert.ToBase64String(KeysManager.AESkey));
            Log("Server Generated RSA Public Key", KeysManager.RSAPublicKey);
            Log();


            server.respose = (s) => {

                string requstType = s.ReadString();
                ResposesManager.ProcessRequst(requstType,s,this);

            };
            
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


        public void Log()
        {
            Log("---------------------------------------------------------------------------------------------------\n");
        }

        public void Log(string logMessage)
        {
            Application.Current.Dispatcher.Invoke((Action)(() => { logTextBox.Text += logMessage+"\n\n"; }));
        }


        public void Log(string messageDescription, string logMessage)
        {
            string format = "{0}:\n{1}\n\n";
            Application.Current.Dispatcher.Invoke((Action)(() => { logTextBox.Text += string.Format(format, messageDescription, logMessage); }));
        }
    }
}
