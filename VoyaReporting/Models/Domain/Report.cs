using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VoyaReporting.Models.Domain
{
    [BsonIgnoreExtraElements]
    public class Report
    {
        public Report()
        {
            HeaderColumns = new List<string>();
            ContentColumns = new List<string>();
        }

        public ObjectId Id { get; set; }

        public string Name { get; set; }
        
        public string Slug { get; set; }

        public string DataSet { get; set; }
        public string TemplateName { get; set; }

        /// <summary>
        /// CSV-based columns in the export of Domo that contain
        /// slugified keys for which columns to include as header
        /// in the Report (non-repeating rows)
        /// </summary>
        public List<string> HeaderColumns { get; set; }

        /// <summary>
        /// Similar as <see cref="HeaderColumns"/>, but for content
        /// instead (repeating rows)
        /// </summary>
        public List<string> ContentColumns { get; set; }
    }
}
