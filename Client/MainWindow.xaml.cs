using Client.Algorithms;
using Client.Util;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Client
{
    public delegate void Logger(string message);
    public partial class MainWindow : Window
    {
        private static string IP = "127.0.0.1";
        private static int HOST = 13000;
        private static int CA = 12000;
        private ClientObject clientObject;
        public static ClientObject clientForCertificate;
        public static Server.Models.Client user;


        public MainWindow()
        {
            instance = this;

            InitializeComponent();

            clientObject = new ClientObject();

            clientObject.initClient(IP, HOST);

            clientForCertificate = new ClientObject();
            clientForCertificate.initClient(IP, CA);

            Log("Clinet started");
            Log("Wating certificate");

            KeyManager.generateSessionKey();

            clientObject.connectUntilSuss((e) =>
            {

                RequestsManager.GetPublicKey(e);
            });


        }


        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            if (user == null)
            {
                // Login
                LoginObject loginObeject = new LoginObject(loginUsernameTextBox.Text, loginPasswordTextBox.Password);
                Server.Models.Client loginClient = RequestsManager.Login(clientObject.stream, loginObeject.toJsonObject());

                if (loginClient != null)
                {
                    user = loginClient;
                    TransactionsTab.Visibility = Visibility.Visible;
                    checkAll.Visibility = Visibility.Visible;

                    
                    loginButton.Content = "SignOut";
                    loginUsernameTextBox.Text = "";
                    loginPasswordTextBox.Password = "";
                }
                else
                {
                    checkAll.Visibility = Visibility.Hidden;
                    TransactionsTab.Visibility = Visibility.Hidden;
                }
            }
            else {
                // Sign out
                TransactionsTab.Visibility = Visibility.Hidden;
                checkAll.Visibility = Visibility.Hidden;
                loginButton.Content = "Login";
                loginUsernameTextBox.Text = "";
                loginPasswordTextBox.Password = "";
            }
        }

        private void transferButton_Click(object sender, RoutedEventArgs e)
        {
            if (user != null)
            {
                TransactionObject transactionObject = new TransactionObject(user.Id.ToString(),
                   reciverIDTextBox.Text.ToString(),
                    ammountTextBox.Text.ToString());

                int selected = trasferAlgorithem.SelectedIndex;
                if (selected == 0)
                {
                    // RSA
                    RequestsManager.TransferWithRSA(clientObject.stream, transactionObject.toJsonObject());
                }
                else {
                    RequestsManager.TransferWithPGP(clientObject.stream, transactionObject.toJsonObject());
                }
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

        private void signUpSubmitButton_Click(object sender, RoutedEventArgs e)
        {
            SignUpObject signUpObject = new SignUpObject(signUpNameTextBox.Text,signUpUsernameTextBox.Text, signUpPasswordTextBox.Password);
            bool signUpResult = RequestsManager.SignUp(clientObject.stream, signUpObject.toJsonObject());
            if (signUpResult)
            {
                TransactionsTab.Visibility = Visibility.Visible;
                checkAll.Visibility = Visibility.Visible;
                loginButton.Content = "SignOut";
                loginUsernameTextBox.Text = "";
                loginPasswordTextBox.Password = "";

                signUpNameTextBox.Text = "";
                signUpPasswordTextBox.Password = "";
                signUpUsernameTextBox.Text = "";
            }
            else
            {
                TransactionsTab.Visibility = Visibility.Hidden;
                checkAll.Visibility = Visibility.Hidden;
            }
        }


        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (LoginTab.IsSelected)
            {
                if (user != null) {
                    loginButton.Content = "SignOut";
                    loginUsernameTextBox.Text = "";
                    loginPasswordTextBox.Password = "";
                }
            }
        }


        private void trasferAlgorithem_Loaded(object sender, RoutedEventArgs e)
        {

            List<string> data = new List<string>();
            data.Add("RSA");
            data.Add("PGP");

            var comboBox = sender as ComboBox;

            comboBox.ItemsSource = data;

            comboBox.SelectedIndex = 0;
        }


        public static MainWindow instance;


        public void Log()
        {
            Log("---------------------------------------------------------------------------------------------------\n");
        }

        public void Log(string logMessage)
        {
            Application.Current.Dispatcher.Invoke((Action)(() => { logTextBox.Text += logMessage + "\n\n"; }));
        }


        public void Log(string messageDescription, string logMessage)
        {
            string format = "{0}:\n{1}\n\n";
            Application.Current.Dispatcher.Invoke((Action)(() => { logTextBox.Text += string.Format(format, messageDescription, logMessage); }));
        }

    }
}
