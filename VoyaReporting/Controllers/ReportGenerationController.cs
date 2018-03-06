using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoyaReporting.Repositories;

namespace VoyaReporting.Controllers
{
    [Route("api/reportGenerations")]
    public class ReportGenerationController : Controller
    {
        private IMongoReportGenerationRepository mongoReportGenerationRepository;

        public ReportGenerationController(IMongoReportGenerationRepository mongoReportGenerationRepository)
        {
            this.mongoReportGenerationRepository = mongoReportGenerationRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetListAsync()
        {
            var lists = await mongoReportGenerationRepository.GetAsync();
            return Ok(lists);
        }
    }
}
