using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoyaReporting.Models.Domain;

namespace VoyaReporting.Repositories
{
    public class MongoReportGenerationRepository : IMongoReportGenerationRepository
    {
        private static readonly string COLLECTION_NAME = "domo_reportingcloud_reportgenerations";
        private IMongoDatabase database;

        public MongoReportGenerationRepository()
        {
            database = new MongoClient(Environment.GetEnvironmentVariable("MONGO_CONNECTIONSTRING"))
                .GetDatabase(Environment.GetEnvironmentVariable("MONGO_DBNAME"));
        }

        private async Task<IMongoCollection<ReportGeneration>> GetCollection()
        {
            var collection = database.GetCollection<ReportGeneration>(COLLECTION_NAME);
            await collection.Indexes.CreateOneAsync(Builders<ReportGeneration>.IndexKeys.Ascending(r => r.Slug));
            await collection.Indexes.CreateOneAsync(Builders<ReportGeneration>.IndexKeys.Ascending(r => r.Name));

            return collection;
        }

        public async Task AddAsync(ReportGeneration report)
        {
            var collection = await GetCollection();
            
            await collection.InsertOneAsync(report);
        }

        public async Task<List<ReportGeneration>> GetAsync()
        {
            var filter = Builders<ReportGeneration>.Filter.Empty;
            
            var sort = Builders<ReportGeneration>.Sort.Descending(r => r.Id);
            var findOptions = new FindOptions<ReportGeneration>
            {
                Sort = sort,
                Skip = 0,
                Limit = 25
            };

            var collection = await GetCollection();
            var cursor = await collection.FindAsync(filter, findOptions);
            var reports = await cursor.ToListAsync();

            return reports;
        }

        public async Task UpdateAsync(ReportGeneration report)
        {
            var builder = Builders<ReportGeneration>.Filter;
            var filter = builder.Eq(r => r.Name, report.Name);

            var updateBuilder = Builders<ReportGeneration>.Update;

            var update = updateBuilder.Set(r => r.Status, report.Status);

            var collection = await GetCollection();
            await collection.FindOneAndUpdateAsync(filter, update);
        }
    }
}
