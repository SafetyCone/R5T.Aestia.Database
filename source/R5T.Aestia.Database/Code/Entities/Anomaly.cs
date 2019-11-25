using System;
using System.Collections.Generic;


namespace R5T.Aestia.Database.Entities
{
    public class Anomaly
    {
        public int ID { get; set; }
        public Guid GUID { get; set; }
        public DateTime ReportedUTC { get; set; }
        public Guid RepotedLocationGUID { get; set; }
        public Guid ReporterLocationGUID { get; set; }

        public ICollection<AnomalyToImageFileMapping> AnomalyToImageFileMappings { get; set; }
        public ICollection<AnomalyToTextItemMapping> AnomalyToTextItemMappings { get; set; }
    }
}
