using System.Data.Entity;

namespace Sentiment.Data.MySql
{
    public interface ITwitterEntities
    {
        int SaveChanges();
        DbSet<source> sources { get; set; }
        DbSet<tweet> tweets { get; set; }
        DbSet<user> users { get; set; }
        DbSet<configinfo> configinfoes { get; set; }
    }
}
