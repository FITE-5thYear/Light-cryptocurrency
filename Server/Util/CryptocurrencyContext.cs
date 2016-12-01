using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
namespace Server.Util
{
    class CryptocurrencyContext : DbContext
    {
        private static string CONNECTION = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=Cryptocurrency";

        public CryptocurrencyContext() : base(CONNECTION)
        {
            Database.SetInitializer(new CryptocurrencyDBInitializer());
            Database.Initialize(true);
        }
        public DbSet<Models.Client> Clients { get; set; }
        public DbSet<Models.Transaction> Transactions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

        }
    }
}
