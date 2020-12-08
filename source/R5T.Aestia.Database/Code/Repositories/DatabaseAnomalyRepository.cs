using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using R5T.Corcyra;
using R5T.Francia;
using R5T.Orgerben;
using R5T.Sindia;
using R5T.Siscia;
using R5T.Venetia;
using R5T.Venetia.Extensions;


namespace R5T.Aestia.Database
{
    public class DatabaseAnomalyRepository<TDbContext> : ProvidedDatabaseRepositoryBase<TDbContext>, IAnomalyRepository
        where TDbContext : DbContext, IAnomalyDbContext
    {
        public DatabaseAnomalyRepository(DbContextOptions<TDbContext> dbContextOptions, IDbContextProvider<TDbContext> dbContextProvider)
            : base(dbContextOptions, dbContextProvider)
        {
        }

        public async Task Add(AnomalyIdentity anomalyIdentity, DateTime reportedUTC)
        {
            await this.ExecuteInContextAsync(async dbContext =>
            {
                var anomalyEntity = new Entities.Anomaly()
                {
                    GUID = anomalyIdentity.Value,
                    ReportedUTC = reportedUTC,
                };

                dbContext.Anomalies.Add(anomalyEntity);

                await dbContext.SaveChangesAsync();
            });
        }

        public async Task<bool> Exists(AnomalyIdentity anomalyIdentity)
        {
            var exists = await this.ExecuteInContextAsync(async dbContext =>
            {
                var anomalyEntity = await dbContext.Anomalies.Where(x => x.GUID == anomalyIdentity.Value).SingleOrDefaultAsync();

                var anomalyExists = !(anomalyEntity == default);
                return anomalyExists;
            });

            return exists;
        }

        public async Task AddImageFile(AnomalyIdentity anomalyIdentity, ImageFileIdentity imageFileIdentity)
        {
            await this.ExecuteInContextAsync(async dbContext =>
            {
                var anomalyID = await dbContext.GetAnomaly(anomalyIdentity).Select(x => x.ID).SingleAsync();

                // Acquire the AnomalyToImageFileMapping (since there currently can only be one image per anomaly).
                var anomalyToImageFileMappingEntity = await dbContext.AnomalyToImageFileMappings.Where(x => x.AnomalyID == anomalyID).SingleOrDefaultAsync();
                var existsAlready = anomalyToImageFileMappingEntity is object;
                if (existsAlready)
                {
                    anomalyToImageFileMappingEntity.ImageFileGUID = imageFileIdentity.Value;
                }
                else
                {
                    anomalyToImageFileMappingEntity = new Entities.AnomalyToImageFileMapping()
                    {
                        AnomalyID = anomalyID,
                        ImageFileGUID = imageFileIdentity.Value,
                    };

                    dbContext.AnomalyToImageFileMappings.Add(anomalyToImageFileMappingEntity);
                }

                await dbContext.SaveChangesAsync();
            });
        }

        public async Task<List<ImageFileIdentity>> GetImageFiles(AnomalyIdentity anomalyIdentity)
        {
            var imageFileIdentities = await this.ExecuteInContextAsync(async dbContext =>
            {
                var imageFileGuids = dbContext.AnomalyToImageFileMappings.Where(x => x.Anomaly.GUID == anomalyIdentity.Value).Select(x => x.ImageFileGUID);

                var output = await imageFileGuids.Select(x => ImageFileIdentity.From(x)).ToListAsync(); // Execute now.
                return output;
            });

            return imageFileIdentities;
        }

        public async Task<DateTime> GetReportedUTC(AnomalyIdentity anomalyIdentity)
        {
            var reportedUtc = await this.ExecuteInContext(async dbContext =>
            {
                var reportedUtcValue = await dbContext.GetAnomaly(anomalyIdentity)
                    .Select(x => x.ReportedUTC)
                    .SingleOrDefaultAsync();
              
                return reportedUtcValue;
            });

            return reportedUtc;
        }

        public async Task<LocationIdentity> GetReportedLocation(AnomalyIdentity anomalyIdentity)
        {
            var locationIdentity = await this.ExecuteInContext(async dbContext =>
            {
                var locationIdentityValue = await dbContext.GetAnomaly(anomalyIdentity).Select(x => x.ReportedLocationGUID).SingleOrDefaultAsync();

                var output = locationIdentityValue == null ? null : LocationIdentity.From(locationIdentityValue.Value);
              
                return output;
            });

            return locationIdentity;
        }

        public Task<LocationIdentity> GetReporterLocation(AnomalyIdentity anomalyIdentity)
        {
            throw new NotImplementedException();
        }

        public async Task<TextItemIdentity> GetTextItem(AnomalyIdentity anomalyIdentity, TextItemTypeIdentity textItemTypeIdentity)
        {
            var textItemIdentity = await this.ExecuteInContextAsync(async dbContext =>
            {
                var output = await dbContext.AnomalyToTextItemMappings
                    .Where(x => x.Anomaly.GUID == anomalyIdentity.Value && x.TextItemTypeGUID == textItemTypeIdentity.Value)
                    .Select(x => TextItemIdentity.From(x.TextItemGUID))
                    .SingleAsync();

                return output;
            });

            return textItemIdentity;
        }

        public Task<List<Tuple<TextItemTypeIdentity, TextItemIdentity>>> GetTextItems(AnomalyIdentity anomalyIdentity)
        {
            throw new NotImplementedException();
        }

        public Task<(bool HasReporterLocation, LocationIdentity LocationIdentity)> HasReporterLocation(AnomalyIdentity anomalyIdentity)
        {
            throw new NotImplementedException();
        }

        public async Task SetReportedUTC(AnomalyIdentity anomalyIdentity, DateTime dateTime)
        {
            await this.ExecuteInContextAsync(async dbContext =>
            {
                var entity = await dbContext.GetAnomaly(anomalyIdentity).SingleAsync();

                entity.ReportedUTC = dateTime;

                await dbContext.SaveChangesAsync();
            });
        }

        public async Task SetReportedLocation(AnomalyIdentity anomalyIdentity, LocationIdentity reportedLocation)
        {
            await this.ExecuteInContextAsync(async dbContext =>
            {
                var entity = await dbContext.GetAnomaly(anomalyIdentity).SingleAsync();

                entity.ReportedLocationGUID = reportedLocation.Value;

                await dbContext.SaveChangesAsync();
            });
        }

        public Task SetReporterLocation(AnomalyIdentity anomalyIdentity, LocationIdentity reporterLocation)
        {
            throw new NotImplementedException();
        }

        public async Task SetTextItem(AnomalyIdentity anomalyIdentity, TextItemTypeIdentity textItemTypeIdentity, TextItemIdentity textItemIdentity)
        {
            await this.ExecuteInContextAsync(async dbContext =>
            {
                var anomalyID = await dbContext.GetAnomaly(anomalyIdentity).Select(x => x.ID).SingleAsync();

                var count = await dbContext.AnomalyToTextItemMappings.Where(x => x.Anomaly.GUID == anomalyIdentity.Value && x.TextItemTypeGUID == textItemTypeIdentity.Value).CountAsync();

                var alreadyExists = count > 0;
                if (alreadyExists)
                {
                    var mappingEntity = await dbContext.AnomalyToTextItemMappings.Where(x => x.Anomaly.GUID == anomalyIdentity.Value && x.TextItemTypeGUID == textItemTypeIdentity.Value).SingleAsync();

                    mappingEntity.TextItemGUID = textItemIdentity.Value;
                }
                else
                {
                    var mappingEntity = new Entities.AnomalyToTextItemMapping()
                    {
                        AnomalyID = anomalyID,
                        TextItemTypeGUID = textItemTypeIdentity.Value,
                        TextItemGUID = textItemIdentity.Value,
                    };

                    dbContext.AnomalyToTextItemMappings.Add(mappingEntity);
                }

                await dbContext.SaveChangesAsync();
            });
        }

        public async Task<bool> ExistsTextItem(AnomalyIdentity anomalyIdentity, TextItemTypeIdentity textItemTypeIdentity)
        {
            var exists = await this.ExecuteInContextAsync(async dbContext =>
            {
                var count = await dbContext.AnomalyToTextItemMappings.Where(x => x.Anomaly.GUID == anomalyIdentity.Value && x.TextItemTypeGUID == textItemTypeIdentity.Value).CountAsync();

                var output = count > 0;
                return output;
            });

            return exists;
        }

        /// <summary>
        /// Returns the single catchment identity that the anomaly is currently mapped to.
        /// </summary>
        /// <param name="anomalyIdentity"></param>
        /// <returns></returns>
        public async Task<(bool HasCatchment, CatchmentIdentity CatchmentIdentity)> HasCatchment(AnomalyIdentity anomalyIdentity)
        {
            var hasOutput = await this.ExecuteInContext(async dbContext =>
            {
                var output = await dbContext.AnomalyToCatchmentMappings.HasSingleAsync(x => x.Anomaly.GUID == anomalyIdentity.Value);
                //var output = await dbContext.AnomalyToCatchmentMappings.Include(x => x.Anomaly).HasSingleAsync(x => x.Anomaly.GUID.Value == anomalyIdentity.Value);
                //var output = dbContext.AnomalyToCatchmentMappings.Where(x => x.Anomaly.GUID.Value == anomalyIdentity.Value).
                return output;
            });

            var catchmentIdentity = hasOutput.Exists ? CatchmentIdentity.From(hasOutput.Result.CatchmentIdentity) : null;

            return (hasOutput.Exists, catchmentIdentity);
        }

        public async Task<List<CatchmentIdentity>> GetCatchments(AnomalyIdentity anomalyIdentity)
        {
            var catchmentIdentities = await this.ExecuteInContext(async dbContext =>
            {
                var output = await dbContext.AnomalyToCatchmentMappings
                    .Where(x => x.Anomaly.GUID == anomalyIdentity.Value)
                    .Select(x => x.CatchmentIdentity)
                    .Select(x => CatchmentIdentity.From(x))
                    .ToListAsync();

                return output;
            });

            return catchmentIdentities;
        }

        public async Task AddCatchment(AnomalyIdentity anomalyIdentity, CatchmentIdentity catchmentIdentity)
        {
            await this.ExecuteInContext(async dbContext =>
            {
                // Old version involved this
                // var mappingEntity = await dbContext.AnomalyToCatchmentMappings.Acquire(dbContext.Anomalies, anomalyIdentity.Value);
                // mappingEntity.CatchmentIdentity = catchmentIdentity.Value;

                // Get the anomaly ID from its GUID
                var anomalyID = await dbContext.GetAnomaly(anomalyIdentity.Value)
                    .Select(x => x.ID)
                    .SingleAsync();

                // If the anomaly and catchment are already associated, do nothing.
                var exists = await dbContext.AnomalyToCatchmentMappings
                    .Where(x => x.AnomalyID == anomalyID)
                    .Where(x => x.CatchmentIdentity == catchmentIdentity.Value)
                    .AnyAsync();
                if (exists)
                {
                    return;
                }

                // Create new entity
                var newEntity = new Entities.AnomalyToCatchmentMapping {
                    AnomalyID = anomalyID,
                    CatchmentIdentity = catchmentIdentity.Value
                };

                // Add it to the database and save
                dbContext.Add(newEntity);
                await dbContext.SaveChangesAsync();
            });
        }

        public async Task<AnomalyInfo> GetAnomalyInfo(AnomalyIdentity anomalyIdentity)
        {
            var anomalyInfo = await this.ExecuteInContextAsync(async dbContext =>
            {
                var gettingAnomalyDetails = dbContext.GetAnomaly(anomalyIdentity)
                    .Select(x => new
                    {
                        x.ReportedUTC,
                        x.ReportedLocationGUID,
                        x.ReporterLocationGUID,
                        x.UpvotesCount,
                    })
                    .SingleAsync();

                var gettingImageFileIdentityValues = dbContext.GetAnomaly(anomalyIdentity)
                    .Join(dbContext.AnomalyToImageFileMappings,
                        anomaly => anomaly.ID,
                        mapping => mapping.AnomalyID,
                        (_, mapping) => mapping.ImageFileGUID)
                    .ToListAsync();

                var gettingTextItemIdentityValues = dbContext.GetAnomaly(anomalyIdentity)
                    .Join(dbContext.AnomalyToTextItemMappings,
                        anomaly => anomaly.ID,
                        mapping => mapping.AnomalyID,
                        (_, mapping) => mapping.TextItemGUID)
                    .ToListAsync();

                var gettingCatchmentMapping = dbContext.GetAnomaly(anomalyIdentity)
                    .Join(dbContext.AnomalyToCatchmentMappings,
                        anomaly => anomaly.ID,
                        mapping => mapping.AnomalyID,
                        (_, mapping) => mapping)
                    .ToListAsync();


                var anomalyDetails = await gettingAnomalyDetails;
                var imageFileIdentityValues = await gettingImageFileIdentityValues;
                var textItemIdentityValues = await gettingTextItemIdentityValues;
                var catchmentMapping = await gettingCatchmentMapping;

                var catchmentIdentities = catchmentMapping.Select(x => CatchmentIdentity.From(x.CatchmentIdentity)).ToList();
                var imageFileIdentities = imageFileIdentityValues.Select(x => ImageFileIdentity.From(x)).ToList();
                var reportedLocation = anomalyDetails.ReportedLocationGUID.HasValue ? LocationIdentity.From(anomalyDetails.ReportedLocationGUID.Value) : null;
                var reporterLocation = anomalyDetails.ReporterLocationGUID.HasValue ? LocationIdentity.From(anomalyDetails.ReporterLocationGUID.Value) : null;
                var reportedUTC = anomalyDetails.ReportedUTC;
                var textItems = textItemIdentityValues.Select(x => TextItemIdentity.From(x)).ToList();
                var upvotesCount = anomalyDetails.UpvotesCount;

                var output = new AnomalyInfo()
                {
                    AnomalyIdentity = anomalyIdentity,
                    CatchmentIdentities = catchmentIdentities,
                    ImageFileIdentities = imageFileIdentities,
                    ReportedLocation = reportedLocation,
                    ReporterLocation = reporterLocation,
                    ReportedUTC = reportedUTC,
                    TextItems = textItems,
                    UpvotesCount = upvotesCount,
                };
                return output;
            });

            return anomalyInfo;
        }

        public async Task<List<AnomalyIdentity>> GetAllAnomaliesInCatchment(CatchmentIdentity catchmentIdentity)
        {
            var anomalies = await this.ExecuteInContextAsync(async dbContext =>
            {
                var anomalyIdentityValues = await dbContext.AnomalyToCatchmentMappings.Where(x => x.CatchmentIdentity == catchmentIdentity.Value).Select(x => x.Anomaly.GUID).ToListAsync();

                var output = anomalyIdentityValues.Select(x => AnomalyIdentity.From(x)).ToList();
                return output;
            });

            return anomalies;
        }

        public async Task SetOrganization(AnomalyIdentity anomalyIdentity, OrganizationIdentity organizationIdentity)
        {
            // TODO: change this into AddOrganization, so that anomalies can have multiple associated organizations...
            // Though...is AnomalyToOrganizationMappings ever used??
            await this.ExecuteInContextAsync(async dbContext =>
            {
                var mappingEntity = await dbContext.AnomalyToOrganizationMappings.Acquire(dbContext.Anomalies, anomalyIdentity.Value);

                mappingEntity.OrganizationIdentity = organizationIdentity.Value;

                await dbContext.SaveChangesAsync();
            });
        }

        public async Task<List<AnomalyInfo>> GetAnomalyInfos(List<AnomalyIdentity> anomalyIdentities)
        {
            var anomalyGuids = anomalyIdentities.Select(x => x.Value).ToList();
            var output = await this.ExecuteInContextAsync(async dbContext =>
            {
                // Get all the info, in rows.
                var query =
                    from anomaly in dbContext.Anomalies
                    where anomaly.GUID != null
                    where anomalyGuids.Contains(anomaly.GUID)
                    join anomalyCatchment in dbContext.AnomalyToCatchmentMappings
                        on anomaly.ID equals anomalyCatchment.AnomalyID into catchmentGroup
                    from c in catchmentGroup.DefaultIfEmpty()
                    join anomalyText in dbContext.AnomalyToTextItemMappings
                        on anomaly.ID equals anomalyText.AnomalyID into textGroup
                    from t in textGroup.DefaultIfEmpty()
                    join anomalyImage in dbContext.AnomalyToImageFileMappings
                        on anomaly.ID equals anomalyImage.AnomalyID into imageGroup
                    from i in imageGroup.DefaultIfEmpty()
                    select new
                    {
                        anomaly.ID,
                        anomaly.GUID,
                        anomaly.ReportedUTC,
                        anomaly.ReportedLocationGUID,
                        anomaly.ReporterLocationGUID,
                        CatchmentIdentity = c == default ? Guid.Empty : c.CatchmentIdentity,
                        TextItemIdentity = t == default ? Guid.Empty : t.TextItemGUID,
                        ImageIdentity = i == default ? Guid.Empty : i.ImageFileGUID,
                    };
                // Group it by anomaly (since we need an AnomalyInfo per AnomalyIdentity passed in)
                var result = await query.ToListAsync();
                var grouped = result.GroupBy(group =>
                        new {
                            group.ID,
                            group.GUID,
                            group.ReportedUTC,
                            group.ReportedLocationGUID,
                            group.ReporterLocationGUID
                        }, group => group);

                // Use the grouping to put together anomaly infos.
                var anomalyInfos = new List<AnomalyInfo>();
                foreach (var entry in grouped)
                {
                    // Console.WriteLine(entry.Key);
                    var images = new HashSet<Guid>();
                    var catchments = new HashSet<Guid>();
                    var textItemSet = new HashSet<Guid>();
                    foreach (var thing in entry)
                    {
                        images.Add(thing.ImageIdentity);
                        catchments.Add(thing.CatchmentIdentity);
                        textItemSet.Add(thing.TextItemIdentity);
                    }

                    var imagesList = images.ToList();
                    var catchmentsList = catchments.ToList();
                    var textItemsList = textItemSet.ToList();

                    // Console.WriteLine($"    Images ({images.Count}):     {string.Join(',',images.ToList())}");
                    // Console.WriteLine($"    Catchments ({catchments.Count}): {string.Join(',', catchments.ToList())}");
                    // Console.WriteLine($"    Text items ({textItemSet.Count}): {string.Join(',', textItemSet.ToList())}");

                    if (entry.Key.GUID == default)
                    {
                        // Unfortunately this also catches anomalies added with an all-zeros guid...
                        // and we have at least one of those (ID 228 as of 2020-07-17)
                        // throw new Exception("Got an anomaly without a GUID");
                    }
                    var anomalyIdentity = new AnomalyIdentity(entry.Key.GUID);
                    var reportedUTC = entry.Key.ReportedUTC;
                    var reportedLocation = entry.Key.ReportedLocationGUID.HasValue ? LocationIdentity.From(entry.Key.ReportedLocationGUID.Value) : null;
                    var reporterLocation = entry.Key.ReporterLocationGUID.HasValue ? LocationIdentity.From(entry.Key.ReporterLocationGUID.Value) : null;
                    var catchmentIdentities = catchmentsList.Select(x => CatchmentIdentity.From(x)).ToList();
                    var imageFileIdentities = imagesList.Select(x => ImageFileIdentity.From(x)).ToList();
                    var textItems = textItemsList.Select(x => TextItemIdentity.From(x)).ToList();
                    var info = new AnomalyInfo
                    {
                        AnomalyIdentity = anomalyIdentity,
                        ReportedUTC = reportedUTC,
                        ReportedLocation = reportedLocation,
                        ReporterLocation = reporterLocation,
                        CatchmentIdentities = catchmentIdentities,
                        ImageFileIdentities = imageFileIdentities,
                        TextItems = textItems
                    };
                    anomalyInfos.Add(info);
                }
                return anomalyInfos;
            });

            return output;
        }

        public Task<Dictionary<AnomalyIdentity, List<ImageFileIdentity>>> GetImageFilesForAnomalies(IEnumerable<AnomalyIdentity> anomalyIdentities)
        {
            return this.ExecuteInContext(async dbContext =>
            {
                var anomalyGuids = anomalyIdentities.Select(x => x.Value).ToList();

                var query = from anomaly in dbContext.Anomalies
                    where anomalyGuids.Contains(anomaly.GUID)
                    join anomalyImage in dbContext.AnomalyToImageFileMappings
                        on anomaly.ID equals anomalyImage.AnomalyID into imageGroup
                    from i in imageGroup.DefaultIfEmpty()
                    select new
                    {
                        anomaly.GUID,
                        ImageIdentity = i == default ? Guid.Empty : i.ImageFileGUID,
                    };

                var result = await query.ToListAsync();

                // Group it by anomaly (since we need an AnomalyInfo per AnomalyIdentity passed in)
                var grouped = result.GroupBy(
                    x => AnomalyIdentity.From(x.GUID),
                    x => ImageFileIdentity.From(x.ImageIdentity));

                var output = grouped.ToDictionary(
                    grouping => grouping.Key,
                    grouping => grouping
                        .Where(x => !x.IsEmpty())
                        .ToList());

                return output;
            });
        }

        public async Task<int> GetUpvotesCount(AnomalyIdentity anomalyIdentity)
        {
            var upvotesCount = await this.ExecuteInContext(async dbContext =>
            {
                var output = await dbContext.GetAnomaly(anomalyIdentity)
                    .Select(x => x.UpvotesCount)
                    .SingleOrDefaultAsync();
              
                return output;
            });

            return upvotesCount;
        }

        public async Task IncrementUpvotesCount(AnomalyIdentity anomalyIdentity)
        {
            await this.ExecuteInContext(async dbContext =>
            {
                var anomalyEntity = await dbContext.GetAnomaly(anomalyIdentity).SingleAsync();
                anomalyEntity.UpvotesCount += 1;
                await dbContext.SaveChangesAsync();
            });
        }
    }
}
