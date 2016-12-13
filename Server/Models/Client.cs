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

        public Client() { }

        public Client(string name, string username, string password) {
            this.Name = name;
            this.Username = username;
            this.Password = password;
            this.Balance = 0;
        }

        public string toJsonObject()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static Client newClientObject(string jsonObject)
        {
            return JsonConvert.DeserializeObject<Client>(jsonObject);
        }

        override public string ToString()
        {
            return Name;
        }
    }
}
