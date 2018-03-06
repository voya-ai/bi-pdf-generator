using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VoyaReporting.Models.Domo
{
    public class DataSet
    {
        public string id { get; set; }
        public string name { get; set; }
        public int rows { get; set; }
        public int columns { get; set; }
        public Schema schema { get; set; }
        public string dataCurrentAt { get; set; }
        public string createdAt { get; set; }
        public string updatedAt { get; set; }
    }
}
