using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Client.Models
{
    class DigitalCertificate
    {
        public static string IssuerName = "Certification Authority";
        public int SerialNumber;
        public string SubjectName;
        public string SubjectPublicKey;
        public DateTime IssuingDate;
        public byte[] DigitalSignature;

        public DigitalCertificate() { }
        public DigitalCertificate(int number, string Name, DateTime Time, string PublicKey)
        {
            SerialNumber = number;
            SubjectName = Name;
            SubjectPublicKey = PublicKey;
            IssuingDate = Time;
        }


        
        public override string ToString()
        {
            string s;
            s = "Issuer Name: " + IssuerName + "\n" + "Owner: " + SubjectName + "\n"
                + "Owner public key: " + SubjectPublicKey + "\n" +
                "Date validate :" + IssuingDate.Date.ToString()
               // + "\nsignauture: " + Encoding.UTF8.GetString(DigitalSignature);
                ;
            return s;
        }
        public string toJsonObject()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static DigitalCertificate newClientObject(string jsonObject)
        {

            return JsonConvert.DeserializeObject<DigitalCertificate>(jsonObject);
        }
    }
}
