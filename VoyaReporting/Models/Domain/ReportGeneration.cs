using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VoyaReporting.Models.Domain
{
    [BsonIgnoreExtraElements]
    public class ReportGeneration
    {
        public ObjectId Id { get; set; }

        public string Name { get; set; }
        public string Slug { get; set; }
        public string Status { get; set; }
    }
}
