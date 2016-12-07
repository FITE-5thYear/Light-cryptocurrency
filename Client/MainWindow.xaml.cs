using Client.Util;
using Server.Models;
using Server.Util;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Client
{
    public delegate void Logger(string message);
    public partial class MainWindow : Window
    {
        private static string IP = "127.0.0.1";
        private static int HOST = 13000;
        private ClientObject clientObject;

        public static Server.Models.Client user;
        

        public MainWindow()
        {
            instance = this;
            InitializeComponent();

            clientObject = new ClientObject();

            clientObject.initClient(IP, HOST);

            clientObject.connectUntilSuss((e)=> {

                RequestsManager.GetPublicKey(e);
            });
        }

        public void Log(string logMessage)
        {
            Application.Current.Dispatcher.Invoke((Action)(() => {
                logTextBox.Text += logMessage +"\n";
            }));
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e) { }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginObject loginObeject = new LoginObject(usernameTextBox.Text, passwordTextBox.Text);
            RequestsManager.Login(clientObject.stream,loginObeject.toJsonObject());    
        }

        private void transferButton_Click(object sender, RoutedEventArgs e)
        {
            if (user != null)
            {
                TransactionObject transactionObject = new TransactionObject(user.Id.ToString(),
                   reciverIDTextBox.Text.ToString(),
                    ammountTextBox.Text.ToString());

                RequestsManager.Transfer(clientObject.stream, transactionObject.toJsonObject());
            }
            else
            {
                MessageBox.Show("You need to login first");
            }
        }

        private void checkButton_Click(object sender, RoutedEventArgs e)
        {
            RequestsManager.ViewAllAccounts(clientObject.stream);
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            clientObject.close();
        }


        public static MainWindow instance;
    }
}
