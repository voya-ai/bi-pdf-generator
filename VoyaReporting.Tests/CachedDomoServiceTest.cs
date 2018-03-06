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
    public class CachedDomoServiceTest
    {
        [TestMethod]
        public async Task CachedDomoService_GetDataAsync_UsesCacheAsync()
        {
            var domoCache = new Mock<IDomoCache>();
            var domoRepository = new Mock<IDomoRepository>();
            var domoDataGroupingService = new Mock<IDomoDataGroupingService>();

            domoCache.Setup(m => m.GetDataCacheAsync(It.IsAny<string>()))
                .ReturnsAsync(@"Voya,ABC123,2017-05-05
            Voya,DEF456,2017-08-08
            Voya,GHI789,2017-09-09");

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

            domoRepository.Setup(m => m.GetDataAsync(It.IsAny<Report>()))
                .ReturnsAsync(string.Empty);

            var domoService = new CachedDomoService(domoRepository.Object,
                domoDataGroupingService.Object,
                domoCache.Object);
            
            await domoService.GetDataAsync(new Models.Domain.Report()
            {
                DataSet = "123-abc",
                ContentColumns = new List<string>()
                {
                    "reference"
                },
                HeaderColumns = new List<string>()
                {
                    "company-name"
                }
            }, new GenerationSettings());

            domoRepository.Verify(m => m.GetDataAsync(It.IsAny<Report>()), Times.Never);

            domoCache.Verify(m => m.GetDataCacheAsync(It.Is<string>(s => s == "123-abc")));
            domoCache.Verify(m => m.SetDataCacheAsync(It.Is<string>(s => s == "123-abc"), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task CachedDomoService_GetDataAsync_FillsCacheAsync()
        {
            var domoCache = new Mock<IDomoCache>();
            var domoRepository = new Mock<IDomoRepository>();
            var domoDataGroupingService = new Mock<IDomoDataGroupingService>();

            domoCache.Setup(m => m.GetDataCacheAsync(It.IsAny<string>()))
                .ReturnsAsync(string.Empty);
            domoCache.Setup(m => m.SetDataCacheAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            string response = @"Voya,ABC123,2017-05-05
            Voya,DEF456,2017-08-08
            Voya,GHI789,2017-09-09";

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

            domoRepository.Setup(m => m.GetDataAsync(It.IsAny<Report>()))
                .ReturnsAsync(response);

            var domoService = new CachedDomoService(domoRepository.Object,
                domoDataGroupingService.Object,
                domoCache.Object);

            await domoService.GetDataAsync(new Report()
            {
                DataSet = "123-abc",
                ContentColumns = new List<string>()
                {
                    "reference"
                },
                HeaderColumns = new List<string>()
                {
                    "company-name"
                }
            }, new GenerationSettings());

            domoRepository.Verify(m => m.GetDataAsync(It.IsAny<Report>()), Times.Once);
            
            domoCache.Verify(m => m.GetDataCacheAsync(It.Is<string>(s => s == "123-abc")));
            domoCache.Verify(m => m.SetDataCacheAsync(It.Is<string>(s => s == "123-abc"), It.Is<string>(s => s == response)));
        }
    }
}
