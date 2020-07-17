using System;

using Microsoft.EntityFrameworkCore;


namespace R5T.Aestia.Database
{
    public interface IAnomalyDbContext
    {
        DbSet<Entities.Anomaly> Anomalies { get; }
        DbSet<Entities.AnomalyToCatchmentMapping> AnomalyToCatchmentMappings { get; }
        DbSet<Entities.AnomalyToImageFileMapping> AnomalyToImageFileMappings { get; }
        DbSet<Entities.AnomalyToTextItemMapping> AnomalyToTextItemMappings { get; }
    }
}
