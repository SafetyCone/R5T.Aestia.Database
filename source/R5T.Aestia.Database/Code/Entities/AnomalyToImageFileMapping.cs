using System;


namespace R5T.Aestia.Database.Entities
{
    public class AnomalyToImageFileMapping
    {
        public int ID { get; set; }

        public int AnomalyID { get; set; }
        public Anomaly Anomaly { get; set; }

        public Guid ImageFileGUID { get; set; }
    }
}
