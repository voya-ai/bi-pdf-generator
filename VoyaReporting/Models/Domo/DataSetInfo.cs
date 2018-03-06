namespace VoyaReporting.Models.Domo
{
    public class DataSetInfo
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int rows { get; set; }
        public int columns { get; set; }
        public string dataCurrentAt { get; set; }
        public string createdAt { get; set; }
        public string updatedAt { get; set; }
        public bool pdpEnabled { get; set; }
    }
}
