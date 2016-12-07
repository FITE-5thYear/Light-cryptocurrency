using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    [Serializable]
    public  class TransactionObject
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

        public static TransactionObject newLoginObject(string jsonObject)
        {
            return JsonConvert.DeserializeObject<TransactionObject>(jsonObject);
        }
    }
}
