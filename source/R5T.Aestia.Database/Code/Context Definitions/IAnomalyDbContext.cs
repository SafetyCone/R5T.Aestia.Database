using System;

using Microsoft.EntityFrameworkCore;


namespace R5T.Aestia.Database
{
    public interface IAnomalyDbContext
    {
        DbSet<Entities.Anomaly> Anomalies { get; set; }
        DbSet<Entities.AnomalyToCatchmentMapping> AnomalyToCatchmentMappings { get; set; }
        DbSet<Entities.AnomalyToImageFileMapping> AnomalyToImageFileMappings { get; set; }
        DbSet<Entities.AnomalyToTextItemMapping> AnomalyToTextItemMappings { get; set; }
    }
}
