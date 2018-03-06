using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VoyaReporting.Repositories;
using VoyaReporting.Models.Domo;
using VoyaReporting.Services;

namespace VoyaReporting.Controllers
{
    [Route("api/datasets")]
    public class DataSetController : Controller
    {
        private IDomoService domoService;

        public DataSetController(IDomoService domoService)
        {
            this.domoService = domoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetListAsync()
        {
            var datasets = await domoService.GetDataSetsAsync();
            return Ok(datasets);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(string id)
        {
            var dataSet = await domoService.GetInfoAsync(id);
            return Ok(dataSet);
        }
    }
}
