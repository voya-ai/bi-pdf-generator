using Newtonsoft.Json;
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
    public class CachedDomoService : AbstractDomoService
    {
        private IDomoRepository domoRepository;
        private IDomoDataGroupingService domoDataGroupingService;
        private IDomoCache domoCache;

        public CachedDomoService(IDomoRepository domoRepository,
            IDomoDataGroupingService domoDataGroupingService,
            IDomoCache domoCache)
        {
            this.domoRepository = domoRepository;
            this.domoDataGroupingService = domoDataGroupingService;
            this.domoCache = domoCache;
        }

        public override async Task<GroupedDomoData> GetDataAsync(Report report, GenerationSettings settings)
        {
            var header = await GetDataHeadersAsync(report);

            var cachableResponse = await domoCache.GetDataCacheAsync(report.DataSet);

            if (string.IsNullOrEmpty(cachableResponse))
            {
                cachableResponse = await domoRepository.GetDataAsync(report);

                await domoCache.SetDataCacheAsync(report.DataSet, cachableResponse);
            }

            var data = new DomoCSVParser().Parse(cachableResponse, report, header, settings);
            return domoDataGroupingService.Group(data, report, settings);
        }

        public override async Task<List<DataSetInfo>> GetDataSetsAsync()
        {
            var cacheableDataSets = await domoCache.GetDataSetsCacheAsync();

            if (!string.IsNullOrEmpty(cacheableDataSets))
            {
                return JsonConvert.DeserializeObject<List<DataSetInfo>>(cacheableDataSets);
            }

            var dataSets = await domoRepository.GetDataSetsAsync();

            await domoCache.SetDataSetsCacheAsync(JsonConvert.SerializeObject(dataSets));

            return dataSets;
        }

        public override async Task<DataSet> GetInfoAsync(string dataSet)
        {
            return await domoRepository.GetInfoAsync(dataSet);
        }
    }
}
