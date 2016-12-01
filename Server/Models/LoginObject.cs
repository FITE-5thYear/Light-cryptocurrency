using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    [Serializable]
    class LoginObject
    {

        public LoginObject() { }
        public LoginObject(string username,string password)
        {
            this.username = username;
            this.password = password;
        }

        public string username { set; get; }
        public string password { set; get; }

        public string toJsonObject() {
            return JsonConvert.SerializeObject(this);
        }

        public static LoginObject newLoginObject(string jsonObject){
            return JsonConvert.DeserializeObject<LoginObject>(jsonObject);
        }
    }
}
