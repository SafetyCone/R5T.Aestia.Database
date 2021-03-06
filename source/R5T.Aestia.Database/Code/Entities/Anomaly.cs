﻿using System;
using System.Collections.Generic;

using R5T.Magyar;


namespace R5T.Aestia.Database.Entities
{
    public class Anomaly : IIDed
    {
        public int ID { get; set; }
        public Guid GUID { get; set; }
        public DateTime ReportedUTC { get; set; }
        public Guid? ReportedLocationGUID { get; set; }
        public Guid? ReporterLocationGUID { get; set; }
        public int UpvotesCount { get; set; }

        public ICollection<AnomalyToCatchmentMapping> AnomalyToCatchmentMappings { get; set; }
        public ICollection<AnomalyToImageFileMapping> AnomalyToImageFileMappings { get; set; }
        public ICollection<AnomalyToOrganizationMapping> AnomalyToOrganizationMappings { get; set; }
        public ICollection<AnomalyToTextItemMapping> AnomalyToTextItemMappings { get; set; }
    }
}
