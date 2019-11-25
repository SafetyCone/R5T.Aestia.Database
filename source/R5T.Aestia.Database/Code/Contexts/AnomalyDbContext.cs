using System;

using Microsoft.EntityFrameworkCore;


namespace R5T.Aestia.Database
{
    public class AnomalyDbContext : DbContext
    {
        public DbSet<Entities.Anomaly> Anomalies { get; set; }
        public DbSet<Entities.AnomalyToImageFileMapping> AnomalyToImageFileMappings { get; set; }
        public DbSet<Entities.AnomalyToTextItemMapping> AnomalyToTextItemMappings { get; set; }


        public AnomalyDbContext(DbContextOptions options)
            : base(options)
        {
        }
    }
}
