using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using R5T.Sindia;
using R5T.Venetia.Extensions;

using R5T.Aestia.Database.Entities;


namespace R5T.Aestia.Database
{
    public static class AnomalyToCatchmentMappingDbSetExtensions
    {
        public static async Task<AnomalyToCatchmentMapping> Acquire(this DbSet<AnomalyToCatchmentMapping> set, DbSet<Anomaly> anomalies, Guid anomalyIdentityValue)
        {
            var anomalyToCatchmentMappingEntity = await set.AcquireSingleAsync(
                x => x.Anomaly.GUID == anomalyIdentityValue,
                async () =>
                {
                    var anomalyID = await anomalies.GetIDByPredicateForSingleAsync(x => x.GUID == anomalyIdentityValue);

                    var output = new AnomalyToCatchmentMapping()
                    {
                        AnomalyID = anomalyID
                    };
                    return output;
                });

            return anomalyToCatchmentMappingEntity;
        }
    }
}
