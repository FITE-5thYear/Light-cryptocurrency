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
using System.Windows.Shapes;

namespace Server
{
    /// <summary>
    /// Interaction logic for CreatUserWindows.xaml
    /// </summary>
    public partial class CreatUserWindows : Window
    {
        public CreatUserWindows()
        {
            InitializeComponent();
        }

        private void CancleButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            string userName = UserNameBox.Text;
            string password = PasswordBox.Password;
            int balance;
            if (!Int32.TryParse(InitialBalnceBox.Text, out balance))
            {
                MessageBox.Show("Enter a valid values", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (balance < 0)
            {
                MessageBox.Show("Enter a valid values", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                SqlServer.InsertUser(userName, password, balance);
            }


        }
    }
}
