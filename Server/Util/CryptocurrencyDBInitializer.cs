using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Util
{
    class CryptocurrencyDBInitializer : CreateDatabaseIfNotExists<CryptocurrencyContext>
    {
        protected override void Seed(CryptocurrencyContext context)
        {
            IList<Server.Models.Client> defaultClients = new List<Server.Models.Client>();

            defaultClients.Add(new Server.Models.Client() { Name = "Mohammed Ghanem", Username = "MG", Password = "MG123", Balance = 1000 });
            defaultClients.Add(new Server.Models.Client() { Name = "Bassel Shmali"  , Username = "BS", Password = "BS123", Balance = 1000 });
            defaultClients.Add(new Server.Models.Client() { Name = "Hania Al Malki",  Username = "HM", Password = "HM123", Balance = 500  });
            defaultClients.Add(new Server.Models.Client() { Name = "Aalaa Al Hamwi",  Username = "AH", Password = "AH123", Balance = 500 });
            foreach (Server.Models.Client client in defaultClients)
                context.Clients.Add(client);

            base.Seed(context);
        }
    }
}
