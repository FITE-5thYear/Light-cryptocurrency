using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    public class Client
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }
        public int Balance { get; set; }

        public string toJsonObject()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static Client newClientObject(string jsonObject)
        {
            return JsonConvert.DeserializeObject<Client>(jsonObject);
        }
    }
}
