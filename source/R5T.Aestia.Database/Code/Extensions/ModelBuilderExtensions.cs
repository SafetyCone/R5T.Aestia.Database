using System;

using Microsoft.EntityFrameworkCore;


namespace R5T.Aestia.Database
{
    public static class ModelBuilderExtensions
    {
        public static ModelBuilder ForAnomalyDbContext(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entities.AnomalyToCatchmentMapping>()
                .HasOne(x => x.Anomaly)
                .WithMany(x => x.AnomalyToCatchmentMappings)
                .HasForeignKey(x => x.AnomalyID);

            // For now, can only be one image file per anomaly.
            modelBuilder.Entity<Entities.AnomalyToImageFileMapping>().HasAlternateKey(x => x.AnomalyID);
            modelBuilder.Entity<Entities.AnomalyToImageFileMapping>()
                .HasOne(x => x.Anomaly)
                .WithMany(x => x.AnomalyToImageFileMappings)
                .HasForeignKey(x => x.AnomalyID);

            // For now, can only be one image file per anomaly.
            modelBuilder.Entity<Entities.AnomalyToOrganizationMapping>().HasAlternateKey(x => x.AnomalyID);
            modelBuilder.Entity<Entities.AnomalyToOrganizationMapping>()
                .HasOne(x => x.Anomaly)
                .WithMany(x => x.AnomalyToOrganizationMappings)
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
