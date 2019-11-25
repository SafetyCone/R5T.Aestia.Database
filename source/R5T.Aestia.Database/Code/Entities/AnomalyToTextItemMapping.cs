using System;


namespace R5T.Aestia.Database.Entities
{
    public class AnomalyToTextItemMapping
    {
        public int ID { get; set; }

        public int AnomalyID { get; set;}
        public Anomaly Anomaly { get; set; }

        public Guid TextItemTypeGUID { get; set; }
        public Guid TextItemGUID { get; set; }
    }
}
