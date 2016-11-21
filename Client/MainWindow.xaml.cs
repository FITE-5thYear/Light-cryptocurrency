using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void ConnectToServerButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                client.InitClient();
                ConnectionStatusLabel.Content = "sucessfully connected\n";
                RequestAccButton.Visibility = Visibility.Visible;
            }
            catch (Exception)
            {

                ConnectionStatusLabel.Content = "Failed to connect\n";
            }
        }

        private void RequestAccButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string s = client.RequestAcc();
                ConnectionStatusLabel.Content += "Request is sent\n";
                ConnectionStatusLabel.Content += s;
            }
            catch (Exception exception)
            {
                ConnectionStatusLabel.Content += "Falied to send Request";
                ConnectionStatusLabel.Content += exception.ToString();
            }
        }

    }
}
