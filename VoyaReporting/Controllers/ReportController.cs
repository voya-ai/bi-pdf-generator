using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoyaReporting.Models;
using VoyaReporting.Models.Domain;
using VoyaReporting.Repositories;
using VoyaReporting.Services;

namespace VoyaReporting.Controllers
{
    [Route("api/reports")]
    public class ReportController : Controller
    {
        private IMongoReportRepository mongoReportRepository;
        private IReportGenerationService reportGenerationService;
        private IDomoService domoService;

        public ReportController(IMongoReportRepository mongoReportRepository,
            IReportGenerationService reportGenerationService,
            IDomoService domoService)
        {
            this.mongoReportRepository = mongoReportRepository;
            this.reportGenerationService = reportGenerationService;
            this.domoService = domoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetListAsync()
        {
            var lists = await mongoReportRepository.GetAsync();
            return Ok(lists);
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> GetAsync(string slug)
        {
            var report = await mongoReportRepository.GetAsync(slug);

            if (report == null)
            {
                return NotFound();
            }

            return Ok(report);
        }

        [HttpPost("{slug}/sample")]
        public async Task<IActionResult> SampleAsync(string slug, [FromBody]GenerationSettings settings)
        {
            var report = await mongoReportRepository.GetAsync(slug);

            if (report == null)
            {
                return NotFound();
            }

            var data = await domoService.GetDataAsync(report, settings);
            var sampleData = data[data.Keys.ToList()[0]]
                .Take(5)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            var sample = reportGenerationService.ConvertToData(sampleData).ToString();

            return File(Encoding.UTF8.GetBytes(sample), "application/json");
        }

        [HttpPost("{slug}/generate")]
        public async Task<IActionResult> GenerateAsync(string slug, [FromBody]GenerationSettings settings)
        {
            var report = await mongoReportRepository.GetAsync(slug);

            if (report == null)
            {
                return NotFound();
            }

            var url = await reportGenerationService.GenerateAsync(report, settings);
            return Ok(url);
        }

        [HttpPatch()]
        public async Task<IActionResult> UpdateAsync([FromBody]Report report)
        {
            if(await mongoReportRepository.GetAsync(report.Slug) == null)
            {
                return NotFound();
            }

            await mongoReportRepository.UpdateAsync(report);

            return Ok(report);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody]Report report)
        {
            await mongoReportRepository.AddAsync(report);

            return Created(report.Slug, report);
        }
    }
}
