using System;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VoyaReporting.Repositories;
using System.Collections.Generic;
using VoyaReporting.Controllers;
using System.Linq;
using VoyaReporting.Models.Domain;
using VoyaReporting.Services;
using VoyaReporting.Models;
using System.Threading.Tasks;

namespace VoyaReporting.Tests
{
    [TestClass]
    public class ReportControllerTest
    {
        [TestMethod]
        public async Task ReportController_GenerateAsync_CallsServicesAsync()
        {
            var reportGenerationService = new Mock<IReportGenerationService>();
            var mongoReportRepository = new Mock<IMongoReportRepository>();
            var domoService = new Mock<IDomoService>();

            reportGenerationService.Setup(m => m.GenerateAsync(It.IsAny<Report>(), It.IsAny<GenerationSettings>()))
                .ReturnsAsync("http://www.example.org");

            mongoReportRepository.Setup(m => m.GetAsync(It.IsAny<string>()))
                .ReturnsAsync((string slug) => new Report()
                {
                    TemplateName = "invoice.docx",
                    DataSet = "invoice_data",
                    Slug = slug
                });
            
            var controller = new ReportController(mongoReportRepository.Object,
                reportGenerationService.Object,
                domoService.Object);

            var result = await controller.GenerateAsync("voya-invoices", new GenerationSettings()
            {
                MasterGroup = "master-group"
            });

            mongoReportRepository.Verify(m => m.GetAsync(It.Is<string>(s => s == "voya-invoices")));
            reportGenerationService.Verify(m => m.GenerateAsync(It.Is<Report>(r => r.Slug == "voya-invoices"
                && r.TemplateName == "invoice.docx"
                && r.DataSet == "invoice_data"), It.Is<GenerationSettings>(s => s.MasterGroup == "master-group")));
        }

        [TestMethod]
        public async Task ReportController_SampleAsync_CallsServicesAsync()
        {
            var reportGenerationService = new Mock<IReportGenerationService>();
            var mongoReportRepository = new Mock<IMongoReportRepository>();
            var domoService = new Mock<IDomoService>();

            reportGenerationService.Setup(m => m.ConvertToData(It.IsAny<Dictionary<string,
                Tuple<
                    Dictionary<string, string>,
                    List<Dictionary<string, string>>>>>()))
                .Returns(new Newtonsoft.Json.Linq.JArray());

            mongoReportRepository.Setup(m => m.GetAsync(It.IsAny<string>()))
                .ReturnsAsync((string slug) => new Report()
                {
                    TemplateName = "invoice.docx",
                    DataSet = "invoice_data",
                    Slug = slug
                });

            var data = new GroupedDomoData();
            data.Add("default", new ChildGroupedDomoData());

            domoService.Setup(m => m.GetDataAsync(It.IsAny<Report>(), It.IsAny<GenerationSettings>()))
                .ReturnsAsync(data);

            var controller = new ReportController(mongoReportRepository.Object,
                reportGenerationService.Object,
                domoService.Object);

            var result = await controller.SampleAsync("voya-invoices", new GenerationSettings()
            {
                MasterGroup = "master-group"
            });

            mongoReportRepository.Verify(m => m.GetAsync(It.Is<string>(s => s == "voya-invoices")));
            reportGenerationService.Verify(m => m.ConvertToData(It.IsAny<Dictionary<string,
                Tuple<
                    Dictionary<string, string>,
                    List<Dictionary<string, string>>>>>()));
            domoService.Verify(m => m.GetDataAsync(It.Is<Report>(r => r.Slug == "voya-invoices"), It.IsAny<GenerationSettings>()));
        }
    }
}
