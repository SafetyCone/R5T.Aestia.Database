using System;
using System.Linq;

using R5T.Sindia;

using AnomalyEntity = R5T.Aestia.Database.Entities.Anomaly;


namespace R5T.Aestia.Database
{
    public static class IAnomalyDbContextExtensions
    {
        public static IQueryable<AnomalyEntity> GetAnomaly(this IAnomalyDbContext dbContext, Guid anomalyIdentityValue)
        {
            var anomalyEntityQueryable = dbContext.Anomalies.Where(x => x.GUID == anomalyIdentityValue);
            return anomalyEntityQueryable;
        }

        public static IQueryable<AnomalyEntity> GetAnomaly(this IAnomalyDbContext dbContext, AnomalyIdentity anomalyIdentity)
        {
            var anomalyEntityQueryable = dbContext.GetAnomaly(anomalyIdentity.Value);
            return anomalyEntityQueryable;
        }
    }
}
