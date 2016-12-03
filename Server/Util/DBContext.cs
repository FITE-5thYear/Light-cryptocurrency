using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
namespace Server.Util
{
    public class DBContext : DbContext
    {

        private static DBContext instance;
        private static string CONNECTION = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=Cryptocurrency";

        private DBContext() : base(CONNECTION)
        {
            Database.SetInitializer(new CryptocurrencyDBInitializer());
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
    }
}
