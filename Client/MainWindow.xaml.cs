﻿using Client.Models;
using System;
using System.Net.Sockets;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;

namespace Client
{
    public partial class MainWindow : Window
    {
        public static Client.Models.Client user;
        public MainWindow()
        {
            InitializeComponent();
        }

        public void Log(string logMessage)
        {
            Application.Current.Dispatcher.Invoke((Action)(() => { logTextBox.Text += logMessage; }));
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e) { }

        private void connect_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                UtilitiesMethods.InitClient(this);
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

            if (UtilitiesMethods.tcpClient == null)
            {
                MessageBox.Show("Connect to server first");

            }
            else {

                LoginObject loginObeject = new LoginObject(usernameTextBox.Text, passwordTextBox.Text);
                UtilitiesMethods.login(loginObeject.toJsonObject(), this);
            }

            
        }

        private void transferButton_Click(object sender, RoutedEventArgs e)
        {
            if (user != null)
            {
                TransactionObject transactionObject = new TransactionObject(user.Id.ToString(),
                   reciverIDTextBox.Text.ToString(),
                    ammountTextBox.Text.ToString());

                UtilitiesMethods.transfer(transactionObject.toJsonObject(), this);
            }
            else
            {
                MessageBox.Show("You need to login first");
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void checkButton_Click(object sender, RoutedEventArgs e)
        {
            if (UtilitiesMethods.tcpClient == null)
            {
                MessageBox.Show("Connect to server first");

            }
            else
            {
                UtilitiesMethods.ViewAllAccounts(this);

            }
        }
    }
}
