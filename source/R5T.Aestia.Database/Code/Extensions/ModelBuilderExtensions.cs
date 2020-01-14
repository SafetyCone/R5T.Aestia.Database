using System;

using Microsoft.EntityFrameworkCore;


namespace R5T.Aestia.Database
{
    public static class ModelBuilderExtensions
    {
        public static ModelBuilder ForAnomalyDbContext(this ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Entities.Anomaly>().HasAlternateKey(x => x.GUID); // GUID is nullable, leave this for now.
            modelBuilder.Entity<Entities.AnomalyToCatchmentMapping>().HasAlternateKey(x => x.AnomalyID);
            modelBuilder.Entity<Entities.AnomalyToCatchmentMapping>()
                .HasOne(x => x.Anomaly)
                .WithOne(x => x.AnomalyToCatchmentMapping)
                .HasForeignKey<Entities.AnomalyToCatchmentMapping>(x => x.AnomalyID);

            // For now, can only be one image file per anomaly.
            modelBuilder.Entity<Entities.AnomalyToImageFileMapping>().HasAlternateKey(x => x.AnomalyID);
            modelBuilder.Entity<Entities.AnomalyToImageFileMapping>()
                .HasOne(x => x.Anomaly)
                .WithMany(x => x.AnomalyToImageFileMappings)
                .HasForeignKey(x => x.AnomalyID);

            modelBuilder.Entity<Entities.AnomalyToTextItemMapping>().HasAlternateKey(x => new { x.AnomalyID, x.TextItemTypeGUID });
            modelBuilder.Entity<Entities.AnomalyToTextItemMapping>()
                .HasOne(x => x.Anomaly)
                .WithMany(x => x.AnomalyToTextItemMappings)
                .HasForeignKey(x => x.AnomalyID);

            return modelBuilder;
        }
    }
}
