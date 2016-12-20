using System;
using System.Text;
using System.Windows;
using CertificationAuthorities.Models;


namespace CertificationAuthorities.Util
{
    class RequestsManager
    {
        public static int SerialNumber = 102515588;
        public static void ProcessRequst(string requestType, AdvanceStream stream, MainWindow mainwindow)
        {
            switch (requestType)
            {
                case "0":
                    issueCertificate(stream,mainwindow);
                    break;
                case "2":
                    mainwindow.Log("Server is connected");
                    break;
                case "3":
                    sendPublicKey(stream, mainwindow);
                    break;
                case "100":
                    MainWindow.instance.Log("A client is connected");
                    break;

            }
        }
        private static void issueCertificate(AdvanceStream stream,MainWindow mainwindow)
        {
          
            DateTime time = DateTime.Now;
            string data = stream.ReadString();
            string[] word = data.Split('\t');
            string Name=word[0];
            String PublicKey= word[1];
            DigitalCertificate digitalCertificate = new DigitalCertificate(SerialNumber++, Name, time, PublicKey);
            MessageBoxResult result = MessageBox.Show(digitalCertificate.ToString(), "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Question);


            if (result == MessageBoxResult.OK) 
            {
                stream.Write("1");
                string message = digitalCertificate.ToString();
                stream.Write(digitalCertificate.toJsonObject());

                Application.Current.Dispatcher.Invoke((Action)(() => {
                    mainwindow.CreateCertificatewindows(digitalCertificate);



                }));

            }
            else
            {
                stream.Write("0");
            }
           
        }
        public static void sendPublicKey(AdvanceStream stream,MainWindow mainWindow)
        {
            stream.Write(Algorithms.KeyManager.RSAPublicKey);
            mainWindow.Log("public key is send to server");
        }
        #region Helper

        private static string getString(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }

        private static byte[] getBytes(string st)
        {
            return Encoding.UTF8.GetBytes(st);
        }

        #endregion
    }
}
   
