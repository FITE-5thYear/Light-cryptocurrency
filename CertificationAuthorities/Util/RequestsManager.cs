using System;
using System.Text;
using CertificationAuthorities.Models;


namespace CertificationAuthorities.Util
{
    class RequestsManager
    {
        public static int SerialNumber = 102515588;
        public static void ProcessRequst(string requestType, AdvanceStream stream)
        {
            switch (requestType)
            {
                case "0":
                    issueCertificate(stream);
                    break;
                case "100":
                    MainWindow.instance.Log("A client is connected");
                    break;

            }
        }
        private static void issueCertificate(AdvanceStream stream)
        {
          
            DateTime time = DateTime.Now;
            string data = stream.ReadString();
            string[] word = data.Split('\t');
            string Name=word[0];
            String PublicKey= word[1];
            DigitalCertificate digitalCertificate = new DigitalCertificate(SerialNumber++, Name, time, PublicKey);
            if (CertifacteWindow.showCertificate(digitalCertificate))
            {
                stream.Write("1");
                string message = digitalCertificate.ToString();
                stream.Write(digitalCertificate.toJsonObject());
            }
            else
            {
                stream.Write("0");
            }
           
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
   
