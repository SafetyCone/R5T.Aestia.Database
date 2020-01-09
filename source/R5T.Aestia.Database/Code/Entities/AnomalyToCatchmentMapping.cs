using System;

using R5T.Magyar;


namespace R5T.Aestia.Database.Entities
{
    public class AnomalyToCatchmentMapping : IIDed
    {
        public int ID { get; set; }

        public int AnomalyID { get; set; }
        public Anomaly Anomaly { get; set; }

        public Guid CatchmentIdentity { get; set; }
    }
}
