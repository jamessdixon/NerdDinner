using System.Configuration;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;
using NerdDinner.Models;

namespace NerdDinner.UI.Models
{
    public interface INerdDinners
    {
        Database Database { get; }
        int SaveChanges();
        DbEntityEntry Entry(object entity);

        DbSet<Dinner> Dinners { get; set; }
        DbSet<RSVP> RSVPs { get; set; }
    }

    public class NerdDinners : DbContext, INerdDinners
    {
        public NerdDinners() 
        {
            Configuration.LazyLoadingEnabled = false;
        }

        private static DbConnection GetProfiledConnection()
        {
            return new EFProfiledDbConnection(new SqlConnection(ConfigurationManager.ConnectionStrings["NerdDinners"].ConnectionString),
                                                    MiniProfiler.Current);
        }

        public DbSet<Dinner> Dinners { get; set; }
        public DbSet<RSVP> RSVPs { get; set; }
    }
}