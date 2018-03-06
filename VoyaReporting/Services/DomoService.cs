using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoyaReporting.Models;
using VoyaReporting.Models.Domain;
using VoyaReporting.Models.Domo;
using VoyaReporting.Repositories;

namespace VoyaReporting.Services
{
    public class DomoService : AbstractDomoService
    {
        private IDomoRepository domoRepository;
        private IDomoDataGroupingService domoDataGroupingService;

        public DomoService(IDomoRepository domoRepository,
            IDomoDataGroupingService domoDataGroupingService)
        {
            this.domoRepository = domoRepository;
            this.domoDataGroupingService = domoDataGroupingService;
        }

        public override async Task<GroupedDomoData> GetDataAsync(Report report, GenerationSettings settings)
        {
            var header = await GetDataHeadersAsync(report);
            var response = await domoRepository.GetDataAsync(report);
            var data = new DomoCSVParser().Parse(response, report, header, settings);
            return domoDataGroupingService.Group(data, report, settings);
        }

        public override async Task<List<DataSetInfo>> GetDataSetsAsync()
        {
            return await domoRepository.GetDataSetsAsync();
        }

        public override async Task<DataSet> GetInfoAsync(string dataSet)
        {
            return await domoRepository.GetInfoAsync(dataSet);
        }
    }
}
