using Server.Models;
using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace Server.Util
{
    public class DBContext : DbContext
    {

        private static DBContext instance;
        private static string CONNECTION = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=Cryptocurrency";

        private DBContext() : base(CONNECTION)
        {
            Database.SetInitializer(new DBInitializer());
            Database.Initialize(true);
        }

        public static DBContext getInstace() {
            if (instance == null) {
                instance = new DBContext();
            }
            return instance;
        }

        public DbSet<Models.Client> Clients { get; set; }
        public DbSet<Models.Transaction> Transactions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

        }


        


        public static Boolean DoTransaction(string sender, string reciver, string amount)
        {
            int senderID = int.Parse(sender);
            int reciverID = int.Parse(reciver);


            DBContext db = getInstace();

            int transmitted;
            try
            {
                transmitted = Int32.Parse(amount);
            }
            catch
            {
                transmitted = 0;
            }
            var FromQuery = from t in db.Clients where t.Id == senderID select t;
            var ToQuery = from t in db.Clients where t.Id == reciverID select t;
            int senderBalance = 0;
            int ReciverBalance = 0;
            foreach (Server.Models.Client clinet in FromQuery)
            {
                senderBalance = clinet.Balance;
            }
            foreach (Server.Models.Client clinet in FromQuery)
            {
                ReciverBalance = clinet.Balance;
            }
            if (senderBalance < transmitted)
                return false;
            else
            {
                Server.Models.Client sendeUser = db.Clients.First(e => e.Id.Equals(senderID));
                Server.Models.Client reciverUser = db.Clients.First(e => e.Id.Equals(reciverID));
                reciverUser.Balance += transmitted;
                sendeUser.Balance -= transmitted;
                db.SaveChanges();
                Transaction t = new Transaction();
                t.Amount = transmitted;
                t.ReciverId = reciverUser.Id;
                t.SenderId = sendeUser.Id;
                db.Transactions.Add(t);
                db.SaveChanges();
                return true;
            }
        }
    }
}
