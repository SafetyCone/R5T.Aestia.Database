using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using R5T.Corcyra;
using R5T.Francia;
using R5T.Sindia;
using R5T.Siscia;
using R5T.Venetia;
using R5T.Venetia.Extensions;


namespace R5T.Aestia.Database
{
    public class DatabaseAnomalyRepository<TDbContext> : ProvidedDatabaseRepositoryBase<TDbContext>, IAnomalyRepository
        where TDbContext: DbContext, IAnomalyDbContext
    {
        public DatabaseAnomalyRepository(DbContextOptions<TDbContext> dbContextOptions, IDbContextProvider<TDbContext> dbContextProvider)
            : base(dbContextOptions, dbContextProvider)
        {
        }

        public async Task AddImageFile(AnomalyIdentity anomalyIdentity, ImageFileIdentity imageFile)
        {
            await this.ExecuteInContextAsync(async dbContext =>
            {
                var anomalyID = await dbContext.GetAnomaly(anomalyIdentity).Select(x => x.ID).SingleAsync();

                var anomalyToImageFileMapping = new Entities.AnomalyToImageFileMapping()
                {
                    AnomalyID = anomalyID,
                    ImageFileGUID = imageFile.Value,
                };

                dbContext.AnomalyToImageFileMappings.Add(anomalyToImageFileMapping);

                await dbContext.SaveChangesAsync();
            });
        }

        public async Task<IEnumerable<ImageFileIdentity>> GetImageFiles(AnomalyIdentity anomalyIdentity)
        {
            var imageFileIdentities = await this.ExecuteInContextAsync(async dbContext =>
            {
                var imageFileGuids = dbContext.AnomalyToImageFileMappings.Where(x => x.Anomaly.GUID == anomalyIdentity.Value).Select(x => x.ImageFileGUID);

                var output = await imageFileGuids.Select(x => ImageFileIdentity.From(x)).ToListAsync(); // Execute now.
                return output;
            });

            return imageFileIdentities;
        }

        public Task<DateTime> GetReportedUTC(AnomalyIdentity anomaly)
        {
            throw new NotImplementedException();
        }

        public async Task<LocationIdentity> GetReportedLocation(AnomalyIdentity anomalyIdentity)
        {
            var locationIdentity = await this.ExecuteInContext(async dbContext =>
            {
                var locationIdentityValue = await dbContext.GetAnomaly(anomalyIdentity).Select(x => x.RepotedLocationGUID).SingleAsync();

                var output = LocationIdentity.From(locationIdentityValue.Value);
                return output;
            });

            return locationIdentity;
        }

        public Task<LocationIdentity> GetReporterLocation(AnomalyIdentity anomaly)
        {
            throw new NotImplementedException();
        }

        public Task<TextItemIdentity> GetTextItem(AnomalyIdentity anomaly, TextItemTypeIdentity textItemType)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Tuple<TextItemTypeIdentity, TextItemIdentity>>> GetTextItems(AnomalyIdentity anomaly)
        {
            throw new NotImplementedException();
        }

        public Task<(bool HasReporterLocation, LocationIdentity LocationIdentity)> HasReporterLocation(AnomalyIdentity anomaly)
        {
            throw new NotImplementedException();
        }

        public async Task<AnomalyIdentity> New()
        {
            var anomalyIdentity = AnomalyIdentity.New();

            await this.ExecuteInContextAsync(async dbContext =>
            {
                var anomalyEntity = new Entities.Anomaly()
                {
                    GUID = anomalyIdentity.Value,
                };

                dbContext.Anomalies.Add(anomalyEntity);

                await dbContext.SaveChangesAsync();
            });

            return anomalyIdentity;
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

                entity.RepotedLocationGUID = reportedLocation.Value;

                await dbContext.SaveChangesAsync();
            });
        }

        public Task SetReporterLocation(AnomalyIdentity anomaly, LocationIdentity reporterLocation)
        {
            throw new NotImplementedException();
        }

        public async Task SetTextItem(AnomalyIdentity anomalyIdentity, TextItemTypeIdentity textItemType, TextItemIdentity textItem)
        {
            await this.ExecuteInContextAsync(async dbContext =>
            {
                var anomalyID = await dbContext.GetAnomaly(anomalyIdentity).Select(x => x.ID).SingleAsync();

                var count = await dbContext.AnomalyToTextItemMappings.Where(x => x.Anomaly.GUID == anomalyIdentity.Value && x.TextItemTypeGUID == textItemType.Value).CountAsync();

                var alreadyExists = count > 0;
                if(alreadyExists)
                {
                    var mappingEntity = await dbContext.AnomalyToTextItemMappings.Where(x => x.Anomaly.GUID == anomalyIdentity.Value && x.TextItemTypeGUID == textItemType.Value).SingleAsync();

                    mappingEntity.TextItemGUID = textItem.Value;
                }
                else
                {
                    var mappingEntity = new Entities.AnomalyToTextItemMapping()
                    {
                        AnomalyID = anomalyID,
                        TextItemTypeGUID = textItemType.Value,
                        TextItemGUID = textItem.Value,
                    };

                    dbContext.AnomalyToTextItemMappings.Add(mappingEntity);
                } 

                await dbContext.SaveChangesAsync();
            });
        }

        public async Task<bool> ExistsTextItem(AnomalyIdentity anomaly, TextItemTypeIdentity textItemType)
        {
            var exists = await this.ExecuteInContextAsync(async dbContext =>
            {
                var count = await dbContext.AnomalyToTextItemMappings.Where(x => x.Anomaly.GUID == anomaly.Value && x.TextItemTypeGUID == textItemType.Value).CountAsync();

                var output = count > 0;
                return output;
            });

            return exists;
        }

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

        public async Task<CatchmentIdentity> GetCatchment(AnomalyIdentity anomalyIdentity)
        {
            var catchmentIdentity = await this.ExecuteInContext(async dbContext =>
            {
                var catchmentIdentityValue = await dbContext.AnomalyToCatchmentMappings.Where(x => x.Anomaly.GUID == anomalyIdentity.Value).Select(x => x.CatchmentIdentity).SingleAsync();

                var output = CatchmentIdentity.From(catchmentIdentityValue);
                return output;
            });

            return catchmentIdentity;
        }

        public async Task SetCatchment(AnomalyIdentity anomalyIdentity, CatchmentIdentity catchmentIdentity)
        {
            await this.ExecuteInContext(async dbContext =>
            {
                var mappingEntity = await dbContext.AnomalyToCatchmentMappings.Acquire(dbContext.Anomalies, anomalyIdentity.Value);

                mappingEntity.CatchmentIdentity = catchmentIdentity.Value;

                await dbContext.SaveChangesAsync();
            });
        }

        public async Task<AnomalyInfo> GetAnomalyInfo(AnomalyIdentity anomalyIdentity)
        {
            var anomalyInfo = await this.ExecuteInContext(async dbContext =>
            {
                //var result = await dbContext.GetAnomaly(anomalyIdentity)
                //.Include(x => x.AnomalyToCatchmentMapping)
                //.Include(x => x.AnomalyToImageFileMappings)
                //.Include(x => x.AnomalyToTextItemMappings)
                //.Select(x => new
                //{
                //    x.ReportedUTC,
                //    x.RepotedLocationGUID,
                //    x.ReporterLocationGUID,
                //    x.AnomalyToCatchmentMapping.CatchmentIdentity,
                //    ImageFileIdentities = x.AnomalyToImageFileMappings.Select(anomalyToImageFileMapping => anomalyToImageFileMapping.ImageFileGUID),
                //    TextItemIdentities = x.AnomalyToTextItemMappings.Select(anomalyToTextItemMapping => anomalyToTextItemMapping.TextItemGUID),
                //    //ImageFileIdentities = x.AnomalyToImageFileMappings.Where(mapping => mapping.Anomaly.GUID == anomalyIdentity.Value).Select(anomalyToImageFileMapping => anomalyToImageFileMapping.ImageFileGUID),
                //    //TextItemIdentities = x.AnomalyToTextItemMappings.Where(mapping => mapping.Anomaly.GUID == anomalyIdentity.Value).Select(anomalyToTextItemMapping => anomalyToTextItemMapping.TextItemGUID),
                //})
                //.SingleAsync();

                //var result =
                //   (from anomaly in dbContext.Anomalies
                //    join toCatchmentMapping in dbContext.AnomalyToCatchmentMappings on anomaly.ID equals toCatchmentMapping.AnomalyID
                //    join toImageFileMappings in dbContext.AnomalyToImageFileMappings on anomaly.ID equals toImageFileMappings.AnomalyID
                //    join toTextItemMappings in dbContext.AnomalyToTextItemMappings on anomaly.ID equals toTextItemMappings.AnomalyID
                //    where anomaly.GUID == anomalyIdentity.Value
                //    select new
                //    {
                //        anomaly.ReportedUTC
                //        anomaly.GUID,
                //    }).SingleAsync();

                //var imageFileIdentities = result.

                var gettingAnomalyDetails = dbContext.GetAnomaly(anomalyIdentity)
                    .Select(x => new
                    {
                        x.ReportedUTC,
                        x.RepotedLocationGUID,
                        x.ReporterLocationGUID,
                        x.AnomalyToCatchmentMapping.CatchmentIdentity,
                        //ImageFileIdentities = x.AnomalyToImageFileMappings.Select(anomalyToImageFileMapping => anomalyToImageFileMapping.ImageFileGUID),
                        //TextItemIdentities = x.AnomalyToTextItemMappings.Select(anomalyToTextItemMapping => anomalyToTextItemMapping.TextItemGUID),
                        //ImageFileIdentities = x.AnomalyToImageFileMappings.Where(mapping => mapping.Anomaly.GUID == anomalyIdentity.Value).Select(anomalyToImageFileMapping => anomalyToImageFileMapping.ImageFileGUID),
                        //TextItemIdentities = x.AnomalyToTextItemMappings.Where(mapping => mapping.Anomaly.GUID == anomalyIdentity.Value).Select(anomalyToTextItemMapping => anomalyToTextItemMapping.TextItemGUID),
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

                var anomalyDetails = await gettingAnomalyDetails;
                var imageFileIdentityValues = await gettingImageFileIdentityValues;
                var textItemIdentityValues = await gettingTextItemIdentityValues;

                var output = new AnomalyInfo()
                {
                    AnomalyIdentity = anomalyIdentity,
                    CatchmentIdentity = CatchmentIdentity.From(anomalyDetails.CatchmentIdentity),
                    ImageFileIdentities = imageFileIdentityValues.Select(x => ImageFileIdentity.From(x)).ToList(),
                    ReportedLocation = anomalyDetails.RepotedLocationGUID.HasValue ? LocationIdentity.From(anomalyDetails.RepotedLocationGUID.Value) : null,
                    ReporterLocation = anomalyDetails.ReporterLocationGUID.HasValue ? LocationIdentity.From(anomalyDetails.ReporterLocationGUID.Value) : null,
                    ReportedUTC = anomalyDetails.ReportedUTC.Value,
                    TextItems = textItemIdentityValues.Select(x => TextItemIdentity.From(x)).ToList(),
                };
                return output;
            });

            return anomalyInfo;
        }
    }
}
