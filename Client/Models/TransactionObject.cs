using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    [Serializable]
    class TransactionObject
    {
        public TransactionObject() { }
        public TransactionObject(string senderID, string reciverID, string amount)
        {
            this.senderID = senderID;
            this.reciverID = reciverID;
            this.amount = amount;
        }

        public string senderID { set; get; }
        public string reciverID { set; get; }
        public string amount { set; get; }

        public string toJsonObject()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static LoginObject newLoginObject(string jsonObject)
        {
            return JsonConvert.DeserializeObject<LoginObject>(jsonObject);
        }
    }
}
