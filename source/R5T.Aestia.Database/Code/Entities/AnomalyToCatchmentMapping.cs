using System;


namespace R5T.Aestia.Database.Entities
{
    public class AnomalyToCatchmentMapping
    {
        public int ID { get; set; }

        public int AnomalyID { get; set; }
        public Anomaly Anomaly { get; set; }

        public Guid CatchmentIdentity { get; set; }
    }
}
