using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoyaReporting.Models.Domain;
using VoyaReporting.Util;

namespace VoyaReporting.Repositories
{
    public class MongoReportRepository : IMongoReportRepository
    {
        private static readonly string COLLECTION_NAME = "domo_reportingcloud_reports";
        private IMongoDatabase database;

        public MongoReportRepository()
        {
            database = new MongoClient(Environment.GetEnvironmentVariable("MONGO_CONNECTIONSTRING"))
                .GetDatabase(Environment.GetEnvironmentVariable("MONGO_DBNAME"));
        }

        private async Task<IMongoCollection<Report>> GetCollection()
        {
            var collection = database.GetCollection<Report>(COLLECTION_NAME);
            await collection.Indexes.CreateOneAsync(Builders<Report>.IndexKeys.Ascending(r => r.Slug), new CreateIndexOptions {
                Unique = true
            });
            
            return collection;
        }

        public async Task AddAsync(Report report)
        {
            var collection = await GetCollection();

            report.Slug = Slugify.ToSlug(report.Name);

            var slugNumber = 0;
            while(await GetAsync(report.Slug) != null)
            {
                slugNumber++;
                report.Slug = $"{report.Slug}-{slugNumber}";
            }

            await collection.InsertOneAsync(report);
        }

        public async Task<List<Report>> GetAsync()
        {
            var filter = Builders<Report>.Filter.Empty;
            
            var collection = await GetCollection();
            var cursor = await collection.FindAsync(filter);
            var reports = await cursor.ToListAsync();

            return reports;
        }

        public async Task<Report> GetAsync(string slug)
        {
            var builder = Builders<Report>.Filter;

            var filter = builder.Eq(r => r.Slug, slug);

            var collection = await GetCollection();
            var cursor = await collection.FindAsync(filter);
            var reports = await cursor.ToListAsync();

            return reports.FirstOrDefault();
        }

        public async Task UpdateAsync(Report report)
        {
            var builder = Builders<Report>.Filter;
            var filter = builder.Eq(r => r.Slug, report.Slug);

            var updateBuilder = Builders<Report>.Update;

            var update = updateBuilder.Set(r => r.Name, report.Name);
            update = update.Set(r => r.TemplateName, report.TemplateName);
            update = update.Set(r => r.DataSet, report.DataSet);
            update = update.Set(r => r.HeaderColumns, report.HeaderColumns);
            update = update.Set(r => r.ContentColumns, report.ContentColumns);

            var collection = await GetCollection();
            await collection.FindOneAndUpdateAsync(filter, update);
        }
    }
}
