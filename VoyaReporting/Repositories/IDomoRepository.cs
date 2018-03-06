using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VoyaReporting.Models;
using VoyaReporting.Models.Domain;
using VoyaReporting.Models.Domo;

namespace VoyaReporting.Repositories
{
    public interface IDomoRepository
    {
        Task<List<DataSetInfo>> GetDataSetsAsync();
        Task<string> GetDataAsync(Report report);
        Task<DataSet> GetInfoAsync(string dataSet);
    }
}