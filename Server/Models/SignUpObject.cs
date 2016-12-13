using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    class SignUpObject
    {
        public string name { get; set; }
        public string username { set; get; }
        public string password { set; get; }

        public SignUpObject() { }
        public SignUpObject(string name,string username, string password)
        {
            this.name = name;
            this.username = username;
            this.password = password;
        }

        public string toJsonObject()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static SignUpObject newLoginObject(string jsonObject)
        {
            return JsonConvert.DeserializeObject<SignUpObject>(jsonObject);
        }
    }
}
