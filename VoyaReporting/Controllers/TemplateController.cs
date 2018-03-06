using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VoyaReporting.Repositories;

namespace VoyaReporting.Controllers
{
    [Route("api/templates")]
    public class TemplateController : Controller
    {
        private IReportingCloudRepository reportingCloudRepository;

        public TemplateController(IReportingCloudRepository reportingCloudRepository)
        {
            this.reportingCloudRepository = reportingCloudRepository;
        }

        [HttpGet]
        public IActionResult Get(string name)
        {
            return Ok(reportingCloudRepository.Get(name));
        }

        [HttpGet("{name}")]
        public IActionResult GetOne(string name)
        {
            return Ok(reportingCloudRepository.Get(name).FirstOrDefault());
        }
    }
}
