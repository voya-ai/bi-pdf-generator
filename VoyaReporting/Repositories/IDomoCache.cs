using System.Threading.Tasks;

namespace VoyaReporting.Repositories
{
    public interface IDomoCache
    {
        Task<string> GetDataCacheAsync(string dataSet);
        Task SetDataCacheAsync(string dataSet, string content);
        Task<string> GetDataSetsCacheAsync();
        Task SetDataSetsCacheAsync(string content);
    }
}
