using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace VoyaReporting.Repositories
{
    public class DomoCache : IDomoCache
    {
        private ConnectionMultiplexer multiplexer;
        
        private async Task<ConnectionMultiplexer> GetMultiplexerAsync()
        {
            if(multiplexer == null)
            {
                var port = Environment.GetEnvironmentVariable("REDIS_PORT");
                var dns = await Dns.GetHostAddressesAsync(Environment.GetEnvironmentVariable("REDIS_HOST"));
                var addresses = string.Join(",", dns.Select(x => $"{x.MapToIPv4().ToString()}:{port}"));

                multiplexer = ConnectionMultiplexer.Connect(addresses);
            }

            return multiplexer;
        }

        public async Task<string> GetDataCacheAsync(string dataSet)
        {
            var redis = await GetMultiplexerAsync();
            var db = redis.GetDatabase();
            var value = await db.StringGetAsync($"voyareporting:domo:datasets:{dataSet}");
            return value;
        }

        public async Task SetDataCacheAsync(string dataSet, string content)
        {
            var redis = await GetMultiplexerAsync();
            var db = redis.GetDatabase();
            await db.StringSetAsync($"voyareporting:domo:datasets:{dataSet}",
                content,
                new TimeSpan(hours: 4, minutes: 0, seconds: 0));
        }

        public async Task<string> GetDataSetsCacheAsync()
        {
            var redis = await GetMultiplexerAsync();
            var db = redis.GetDatabase();
            var dataSets = await db.StringGetAsync($"voyareporting:domo:datasets");
            return dataSets;
        }

        public async Task SetDataSetsCacheAsync(string content)
        {
            var redis = await GetMultiplexerAsync();
            var db = redis.GetDatabase();
            await db.StringSetAsync($"voyareporting:domo:datasets",
                content,
                new TimeSpan(hours: 1, minutes: 0, seconds: 0));
        }

    }
}
