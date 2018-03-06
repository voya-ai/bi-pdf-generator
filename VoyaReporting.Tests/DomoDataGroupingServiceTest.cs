using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoyaReporting.Models;
using VoyaReporting.Models.Domain;
using VoyaReporting.Services;

namespace VoyaReporting.Tests
{
    [TestClass]
    public class DomoDataGroupingServiceTest
    {
        private Report report;
        private List<Dictionary<string, string>> data;
        private DomoDataGroupingService domoDataGroupingService;

        public DomoDataGroupingServiceTest()
        {
            report = new Report()
            {
                HeaderColumns = new List<string>()
                {
                    "company-name",
                    "booker",
                    "traveller"
                },
                ContentColumns = new List<string>()
                {
                    "booking-reference",
                    "price"
                }
            };

            data = new List<Dictionary<string, string>>()
            {
                new Dictionary<string, string>()
                {
                    { "company-name", "Voya" },
                    { "booker", "Florian" },
                    { "traveller", "Florian" },
                    { "booking-reference", "123ABC" },
                    { "price", "955.50" }
                },
                new Dictionary<string, string>()
                {
                    { "company-name", "Voya" },
                    { "booker", "Florian" },
                    { "traveller", "Pepijn" },
                    { "booking-reference", "999ABC" },
                    { "price", "60.50" }
                },
                new Dictionary<string, string>()
                {
                    { "company-name", "Eliandor" },
                    { "booker", "Pepijn" },
                    { "traveller", "Pepijn" },
                    { "booking-reference", "SXQ955" },
                    { "price", "800.50" }
                }
            };

            domoDataGroupingService = new DomoDataGroupingService();
        }

        [TestMethod]
        public void DomoDataGroupingService_Group_GroupsByMasterGroup()
        {
            var settings = new GenerationSettings()
            {
                MasterGroup = "company-name",
                ChildGroups = new List<string>()
                {
                    "booker"
                }
            };

            var groupedData = domoDataGroupingService.Group(data, report, settings);

            Assert.AreEqual(2, groupedData.Count);
            Assert.AreEqual("Voya", groupedData.Keys.ToList()[0]);
            Assert.AreEqual("Eliandor", groupedData.Keys.ToList()[1]);
        }

        [TestMethod]
        public void DomoDataGroupingService_Group_UsesDefaultMasterGroup()
        {
            var settings = new GenerationSettings()
            {
                ChildGroups = new List<string>()
                {
                    "booker"
                }
            };

            var groupedData = domoDataGroupingService.Group(data, report, settings);

            Assert.AreEqual(1, groupedData.Count);
            Assert.AreEqual("default", groupedData.Keys.ToList()[0]);
        }
        
        [TestMethod]
        public void DomoDataGroupingService_Group_GroupsChildGroups()
        {
            var settings = new GenerationSettings()
            {
                ChildGroups = new List<string>()
                {
                    "booker"
                }
            };

            var groupedData = domoDataGroupingService.Group(data, report, settings);

            Assert.AreEqual(2, groupedData["default"].Count);
        }

        [TestMethod]
        public void DomoDataGroupingService_Group_GroupsMultipleChildGroups()
        {
            var settings = new GenerationSettings()
            {
                ChildGroups = new List<string>()
                {
                    "booker",
                    "traveller"
                }
            };

            var groupedData = domoDataGroupingService.Group(data, report, settings);

            Assert.AreEqual(3, groupedData["default"].Count);
        }
    }
}
