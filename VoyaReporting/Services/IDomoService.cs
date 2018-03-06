using System.Collections.Generic;
using System.Threading.Tasks;
using VoyaReporting.Models;
using VoyaReporting.Models.Domain;
using VoyaReporting.Models.Domo;

namespace VoyaReporting.Services
{
    public interface IDomoService
    {
        Task<List<DataSetInfo>> GetDataSetsAsync();
        Task<GroupedDomoData> GetDataAsync(Report report, GenerationSettings settings);
        Task<DataSet> GetInfoAsync(string dataSet);
    }
}