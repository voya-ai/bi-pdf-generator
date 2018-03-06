using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoyaReporting.Models;
using VoyaReporting.Models.Domain;
using VoyaReporting.Models.Domo;

namespace VoyaReporting.Services
{
    public abstract class AbstractDomoService : IDomoService
    {
        public abstract Task<GroupedDomoData> GetDataAsync(Report report, GenerationSettings settings);
        public abstract Task<List<DataSetInfo>> GetDataSetsAsync();
        public abstract Task<DataSet> GetInfoAsync(string dataSet);

        public async Task<Dictionary<string, int>> GetDataHeadersAsync(Report report)
        {
            var info = await GetInfoAsync(report.DataSet);

            var fields = info?.schema?.columns
                ?.Select(h => h.name)
                ?.ToList() ?? new List<string>();

            var i = 0;
            var headers = fields.ToDictionary(item => item,
                item =>
                {
                    return i++;
                });

            return headers;
        }
    }
}