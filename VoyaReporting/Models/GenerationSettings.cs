using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VoyaReporting.Models
{
    public class GenerationSettings
    {
        public List<string> ChildGroups { get; set; }
        public string TimeColumn { get; set; }
        public int Timespan { get; set; }
        public string MasterGroup { get; set; }
        public string StartDate { get; set; }
    }
}
