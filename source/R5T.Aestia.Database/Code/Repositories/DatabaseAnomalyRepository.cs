using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using R5T.Corcyra;
using R5T.Francia;
using R5T.Sindia;
using R5T.Siscia;
using R5T.Venetia;


namespace R5T.Aestia.Database
{
    public class DatabaseAnomalyRepository : DatabaseRepositoryBase<AnomalyDbContext>, IAnomalyRepository
    {
        public DatabaseAnomalyRepository(DbContextOptions<AnomalyDbContext> dbContextOptions)
            : base(dbContextOptions)
        {
        }

        public override AnomalyDbContext GetNewDbContext()
        {
            var dbContext = new AnomalyDbContext(this.DbContextOptions);
            return dbContext;
        }

        public void AddImageFile(AnomalyIdentity anomaly, ImageFileIdentity imageFile)
        {
            this.ExecuteInContext(dbContext =>
            {
                var anomalyID = dbContext.Anomalies.Where(x => x.GUID == anomaly.Value).Select(x => x.ID).Single();

                var anomalyToImageFileMapping = new Entities.AnomalyToImageFileMapping()
                {
                    AnomalyID = anomalyID,
                    ImageFileGUID = imageFile.Value,
                };

                dbContext.AnomalyToImageFileMappings.Add(anomalyToImageFileMapping);

                dbContext.SaveChanges();
            });
        }

        public IEnumerable<ImageFileIdentity> GetImageFiles(AnomalyIdentity anomalyIdentity)
        {
            var imageFileIdentities = this.ExecuteInContext(dbContext =>
            {
                var imageFileGuids = dbContext.AnomalyToImageFileMappings.Where(x => x.Anomaly.GUID == anomalyIdentity.Value).Select(x => x.ImageFileGUID);

                var output = imageFileGuids.Select(x => ImageFileIdentity.From(x)).ToList(); // Execute now.
                return output;
            });

            return imageFileIdentities;
        }

        public DateTime GetReportedUTC(AnomalyIdentity anomaly)
        {
            throw new NotImplementedException();
        }

        public LocationIdentity GetReportedLocation(AnomalyIdentity anomaly)
        {
            throw new NotImplementedException();
        }

        public LocationIdentity GetReporterLocation(AnomalyIdentity anomaly)
        {
            throw new NotImplementedException();
        }

        public TextItemIdentity GetTextItem(AnomalyIdentity anomaly, TextItemTypeIdentity textItemType)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tuple<TextItemTypeIdentity, TextItemIdentity>> GetTextItems(AnomalyIdentity anomaly)
        {
            throw new NotImplementedException();
        }

        public bool HasReporterLocation(AnomalyIdentity anomaly, out LocationIdentity reporterLocation)
        {
            throw new NotImplementedException();
        }

        public AnomalyIdentity New()
        {
            var anomalyIdentity = AnomalyIdentity.New();

            using (var dbContext = this.GetNewDbContext())
            {
                var anomalyEntity = new Entities.Anomaly()
                {
                    GUID = anomalyIdentity.Value,
                };

                dbContext.Anomalies.Add(anomalyEntity);

                dbContext.SaveChanges();
            }

            return anomalyIdentity;
        }

        public void SetReportedUTC(AnomalyIdentity anomaly, DateTime dateTime)
        {
            this.ExecuteInContext(dbContext =>
            {
                var entity = dbContext.Anomalies.Where(x => x.GUID == anomaly.Value).Single();

                entity.ReportedUTC = dateTime;

                dbContext.SaveChanges();
            });
        }

        public void SetReportedLocation(AnomalyIdentity anomaly, LocationIdentity reportedLocation)
        {
            throw new NotImplementedException();
        }

        public void SetReporterLocation(AnomalyIdentity anomaly, LocationIdentity reporterLocation)
        {
            throw new NotImplementedException();
        }

        public void SetTextItem(AnomalyIdentity anomaly, TextItemTypeIdentity textItemType, TextItemIdentity textItem)
        {
            throw new NotImplementedException();
        }
    }
}
