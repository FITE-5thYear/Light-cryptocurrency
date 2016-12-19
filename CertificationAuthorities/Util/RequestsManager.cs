using System;
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

            }
        }
        private static void issueCertificate(AdvanceStream stream)
        {
            DateTime time = DateTime.Now;
            string Name = stream.ReadString();
            String PublicKey= stream.ReadString();
            DigitalCertificate digitalCertificate = new DigitalCertificate(SerialNumber++, Name, time, PublicKey);
            string message = digitalCertificate.ToString();
            stream.Write(digitalCertificate.toJsonObject());
           
        }
    }
}
