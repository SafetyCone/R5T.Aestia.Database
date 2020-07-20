using System;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using R5T.Venetia.Extensions;

using R5T.Aestia.Database.Entities;


namespace R5T.Aestia.Database
{
    public static class AnomalyToOrganizationMappingDbSetExtensions
    {
        public static async Task<AnomalyToOrganizationMapping> Acquire(this DbSet<AnomalyToOrganizationMapping> set, DbSet<Anomaly> anomalies, Guid anomalyIdentityValue)
        {
            var anomalyToOrganizationMappingEntity = await set.AcquireSingleAsync(
                x => x.Anomaly.GUID == anomalyIdentityValue,
                async () =>
                {
                    var anomalyID = await anomalies.GetIDByPredicateForSingleAsync(x => x.GUID == anomalyIdentityValue);

                    var output = new AnomalyToOrganizationMapping()
                    {
                        AnomalyID = anomalyID
                    };
                    return output;
                });

            return anomalyToOrganizationMappingEntity;
        }
    }
}
