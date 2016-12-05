using Server.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Client
{
    public partial class MainWindow : Window
    {
        public static Server.Models.Client user;
        public static MainWindow instance;

        public MainWindow()
        {
            instance = this;
            InitializeComponent();
        }

        public void Log(string logMessage)
        {
            Application.Current.Dispatcher.Invoke((Action)(() => {
                logTextBox.Text += logMessage +"\n----------------------------------------------------------------------------------------------\n";
            }));
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e) { }

        private void connect_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                ClientMethods.InitClient();
                connect.IsEnabled = false;
            }
            catch (Exception x)
            {
                connect.IsEnabled = true;
                MessageBox.Show(x.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {

            if (ClientMethods.tcpClient == null)
            {
                MessageBox.Show("Connect to server first");

            }
            else {

                LoginObject loginObeject = new LoginObject(usernameTextBox.Text, passwordTextBox.Text);
                ClientMethods.login(loginObeject.toJsonObject(), this);
            }
        }

        private void transferButton_Click(object sender, RoutedEventArgs e)
        {
            if (user != null)
            {
                TransactionObject transactionObject = new TransactionObject(user.Id.ToString(),
                   reciverIDTextBox.Text.ToString(),
                    ammountTextBox.Text.ToString());

                ClientMethods.transfer(transactionObject.toJsonObject());
            }
            else
            {
                MessageBox.Show("You need to login first");
            }
        }

        private void checkButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClientMethods.tcpClient == null)
            {
                MessageBox.Show("Connect to server first");
            }
            else
            {
                ClientMethods.ViewAllAccounts();
            }
        }
    }
}
