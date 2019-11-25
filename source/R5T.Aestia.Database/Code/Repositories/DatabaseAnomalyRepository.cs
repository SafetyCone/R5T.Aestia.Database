using System;
using System.Collections.Generic;

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
            throw new NotImplementedException();
        }

        public IEnumerable<ImageFileIdentity> GetImageFiles(AnomalyIdentity anomalyIdentity)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void SetReportedUTC(AnomalyIdentity anomaly, DateTime dateTime)
        {
            throw new NotImplementedException();
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
