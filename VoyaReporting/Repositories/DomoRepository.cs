using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoyaReporting.Models.Domain;
using VoyaReporting.Models.Domo;
using VoyaReporting.Util;

namespace VoyaReporting.Repositories
{
    public class DomoRepository : IDomoRepository
    {
        private readonly string username;
        private readonly string password;

        public class AccessToken
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public int expires_in { get; set; }
            public string scope { get; set; }
            public string customer { get; set; }
            public string env { get; set; }
            public int userId { get; set; }
            public string role { get; set; }
            public string jti { get; set; }
        }

        public DomoRepository()
        {
            this.username = Environment.GetEnvironmentVariable("DOMO_USERNAME");
            this.password = Environment.GetEnvironmentVariable("DOMO_PASSWORD");
        }
        
        private async Task<AccessToken> GetTokenAsync()
        {
            return await "https://api.domo.com/oauth/token?grant_type=client_credentials&scope=data"
                .WithBasicAuth(username, password)
                .GetJsonAsync<AccessToken>();
        }

        public async Task<List<DataSetInfo>> GetDataSetsAsync()
        {
            var token = await GetTokenAsync();

            var offset = 0;
            var limit = 50;

            var dataSets = await $"https://api.domo.com/v1/datasets?sort=name&offset=0&limit=50"
                .WithOAuthBearerToken(token.access_token)
                .GetJsonAsync<List<DataSetInfo>>();

            var allDataSets = dataSets;

            while (dataSets.Count >= limit)
            {
                offset += limit;

                dataSets = await $"https://api.domo.com/v1/datasets?sort=name&offset={offset}&limit={limit}"
                    .WithOAuthBearerToken(token.access_token)
                    .GetJsonAsync<List<DataSetInfo>>();

                allDataSets.AddRange(dataSets);
            }
            
            return allDataSets;
        }

        public async Task<DataSet> GetInfoAsync(string dataSet)
        {
            var token = await GetTokenAsync();

            var info = await $"https://api.domo.com/v1/datasets/{dataSet}"
                .WithOAuthBearerToken(token.access_token)
                .GetJsonAsync<DataSet>();

            info.schema.columns = info.schema.columns
                .Select(column => {
                    column.name = Slugify.ToSlug(column.name);
                    return column;
                })
                .Select(column => {
                    int i = 0;
                    while(info.schema.columns.Count(c => c.name == column.name) > 1)
                    {
                        column.name = $"{column.name}-{i}";
                    }
                    return column;
                })
                .ToList();

            return info;
        }
        
        public async Task<string> GetDataAsync(Report report)
        {
            var token = await GetTokenAsync();

            var response = await $"https://api.domo.com/v1/datasets/{report.DataSet}/data"
                .WithOAuthBearerToken(token.access_token)
                .WithHeader("Accept", "text/csv")
                .GetStringAsync();

            return response;
        }
    }
}
