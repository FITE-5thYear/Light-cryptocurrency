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
        public static bool showCertificate(DigitalCertificate dc)
        {
            CertifacteWindow cw = new CertifacteWindow();
            cw.Activate();
            cw.InitializeComponent();
            cw.IssuerNameTB.Text = DigitalCertificate.IssuerName;
            cw.SerialNumberTB.Text = dc.SerialNumber.ToString();
            cw.CertificateOwnerTB.Text = dc.SubjectName.ToString();
            cw.ValidateTB.Text = dc.IssuingDate.Date.ToString();
            cw.OwnerPublicKeyTB.Text = dc.SubjectPublicKey;
            cw.Show();
         
            while (true)
            {
                if (cw.okButtonClicked)
                {
                    cw.Close();
                    return true;
                }
                if(cw.cancleButtonClicked)
                {
                    cw.Close();
                    return false;
                }
            }

        }
    }
}
