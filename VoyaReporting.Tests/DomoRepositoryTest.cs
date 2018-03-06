using Flurl.Http.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoyaReporting.Models;
using VoyaReporting.Models.Domo;
using VoyaReporting.Repositories;

namespace VoyaReporting.Tests
{
    [TestClass]
    public class DomoRepositoryTest
    {
        private HttpTest httpTest;

        [TestInitialize]
        public void Initialize()
        {
            httpTest = new HttpTest();
        }

        [TestCleanup]
        public void Cleanup()
        {
            httpTest.Dispose();
        }

        [TestMethod]
        public async Task DomoRepository_GetDataAsync_FetchesDataAsync()
        {
            var mockDomoCache = new Mock<IDomoCache>();
            mockDomoCache.Setup(m => m.GetDataCacheAsync(It.IsAny<string>()))
                .ReturnsAsync(string.Empty);
            mockDomoCache.Setup(m => m.SetDataCacheAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            httpTest.RespondWithJson(new DomoRepository.AccessToken()
            {
                access_token = "bearer-token"
            });

            httpTest.RespondWithJson(new DataSet()
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

            httpTest.RespondWithJson(new DomoRepository.AccessToken()
            {
                access_token = "bearer-token"
            });

            string response = @"Voya,ABC123,2017-05-05
            Voya,DEF456,2017-08-08
            Voya,GHI789,2017-09-09";

            httpTest.RespondWith(response);

            var domoRepository = new DomoRepository();
            var data = await domoRepository.GetDataAsync(new Models.Domain.Report()
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
            });

            httpTest.ShouldHaveCalled("https://api.domo.com/v1/datasets/123-abc")
                .WithOAuthBearerToken("bearer-token");

            httpTest.ShouldHaveCalled("https://api.domo.com/v1/datasets/123-abc/data")
                .WithOAuthBearerToken("bearer-token");
        }
    }
}
