using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoyaReporting.Models;
using VoyaReporting.Models.Domain;
using VoyaReporting.Models.Domo;
using VoyaReporting.Repositories;
using VoyaReporting.Services;

namespace VoyaReporting.Tests
{
    [TestClass]
    public class DomoServiceTest
    {
        [TestMethod]
        public async Task DomoService_GetDataAsync_CallsServicesAsync()
        {
            var domoRepository = new Mock<IDomoRepository>();
            var domoDataGroupingService = new Mock<IDomoDataGroupingService>();

            domoRepository.Setup(m => m.GetDataAsync(It.IsAny<Report>()))
                .ReturnsAsync(string.Empty);
            domoRepository.Setup(m => m.GetInfoAsync(It.IsAny<string>()))
                .ReturnsAsync(new DataSet());
            domoDataGroupingService.Setup(m => m.Group(It.IsAny<List<Dictionary<string, string>>>(), It.IsAny<Report>(), It.IsAny<GenerationSettings>()))
                .Returns(new GroupedDomoData());

            var domoService = new DomoService(domoRepository.Object,
                domoDataGroupingService.Object);

            await domoService.GetDataAsync(new Report()
            {
                Slug = "123-abc"
            }, new GenerationSettings()
            {
                MasterGroup = "master-group"
            });

            domoRepository.Verify(m => m.GetDataAsync(It.Is<Report>(r => r.Slug == "123-abc")));
            domoDataGroupingService.Verify(m => m.Group(It.IsAny<List<Dictionary<string, string>>>(),
                It.Is<Report>(r => r.Slug == "123-abc"),
                It.Is<GenerationSettings>(s => s.MasterGroup == "master-group")));
        }

        public async Task DomoService_GetDataHeadersAsync_OrdersHeaders()
        {
            var domoRepository = new Mock<IDomoRepository>();
            var domoDataGroupingService = new Mock<IDomoDataGroupingService>();

            domoRepository.Setup(m => m.GetInfoAsync(It.IsAny<string>()))
                .ReturnsAsync(new DataSet()
                {
                    schema = new Schema()
                    {
                        columns = new List<Column>()
                        {
                            new Column()
                            {
                                name = "company-name"
                            },
                            new Column()
                            {
                                name = "reference"
                            },
                            new Column()
                            {
                                name = "date"
                            }
                        }
                    }
                });

            var domoService = new DomoService(domoRepository.Object,
                domoDataGroupingService.Object);

            var headers = await domoService.GetDataHeadersAsync(new Report()
            {
                DataSet = "abc-123"
            });

            Assert.AreEqual(3, headers.Count);
            Assert.AreEqual("company-name", headers.Keys.ToList()[0]);
            Assert.AreEqual("reference", headers.Keys.ToList()[1]);
            Assert.AreEqual("date", headers.Keys.ToList()[2]);
            Assert.AreEqual(0, headers["company-name"]);
            Assert.AreEqual(1, headers["reference"]);
            Assert.AreEqual(2, headers["date"]);
        }
    }
}
