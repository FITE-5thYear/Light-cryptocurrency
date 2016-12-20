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
using CertificationAuthorities.Models;

namespace CertificationAuthorities
{
    /// <summary>
    /// Interaction logic for CertifacteWindow.xaml
    /// </summary>
    public partial class CertifacteWindow : Window
    {
        private bool okButtonClicked;
        private bool cancleButtonClicked;
        public CertifacteWindow()
        {
           InitializeComponent();
            okButtonClicked = false;
            cancleButtonClicked = false;
           
        }

        private void CancleButton_Click(object sender, RoutedEventArgs e)
        {

            okButtonClicked = true;
            CancleButton.IsEnabled = false;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            cancleButtonClicked = true;
            OkButton.IsEnabled = false;
        }
        public bool showCertificate(DigitalCertificate dc)
        {
            
           
           
            this.IssuerNameTB.Text = DigitalCertificate.IssuerName;
            this.SerialNumberTB.Text = dc.SerialNumber.ToString();
            this.CertificateOwnerTB.Text = dc.SubjectName.ToString();
            this.ValidateTB.Text = dc.IssuingDate.Date.ToString();
            this.OwnerPublicKeyTB.Text = dc.SubjectPublicKey;
            this.Visibility = Visibility.Visible;
            this.Show();
         
            while (true)
            {
                if (this.okButtonClicked)
                {
                    
                    return true;
                }
                if(this.cancleButtonClicked)
                {
                   
                    return false;
                }
            }

        }
    }
}
