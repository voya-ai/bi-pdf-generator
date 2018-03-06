using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using VoyaReporting.Models;
using VoyaReporting.Services;

namespace VoyaReporting.Tests
{
    [TestClass]
    public class DomoCSVParserTest
    {
        [TestMethod]
        public void DomoCSVParser_Parse_FiltersData()
        {
            string response = @"Voya,ABC123,2017-05-05
            Voya,DEF456,2017-08-08
            Voya,GHI789,2017-09-09";

            var data = new DomoCSVParser().Parse(response,
                new Models.Domain.Report()
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
                },
                new Dictionary<string, int>()
                {
                    { "company-name", 0 },
                    { "reference", 1 },
                    { "date", 2 }
                },
                new GenerationSettings()
                {
                    StartDate = "2017-08-06",
                    TimeColumn = "date",
                    Timespan = 7
                });

            Assert.AreEqual(1, data.Count);
            Assert.AreEqual("Voya", data[0]["company-name"]);
            Assert.AreEqual("DEF456", data[0]["reference"]);
            Assert.IsFalse(data[0].ContainsKey("date"));
        }
    }
}
