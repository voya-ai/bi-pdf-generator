using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoyaReporting.Models;
using VoyaReporting.Models.Domain;

namespace VoyaReporting.Services
{
    public class DomoDataGroupingService : IDomoDataGroupingService
    {
        public GroupedDomoData Group(List<Dictionary<string, string>> data, Report report, GenerationSettings settings)
        {
            var groupedData = new Dictionary<string, List<Dictionary<string, string>>>();

            if (!string.IsNullOrEmpty(settings.MasterGroup))
            {
                foreach (var row in data)
                {
                    if (!groupedData.ContainsKey(row[settings.MasterGroup]))
                    {
                        groupedData[row[settings.MasterGroup]] = new List<Dictionary<string, string>>();
                    }

                    groupedData[row[settings.MasterGroup]].Add(row);
                }
            }
            else
            {
                groupedData["default"] = data;
            }

            var contentData = new GroupedDomoData();

            foreach (var group in groupedData)
            {
                var groupedContentData = new ChildGroupedDomoData();

                foreach (var row in group.Value)
                {
                    var hash = string.Join("-",
                        settings.ChildGroups.Select(childGroup => row[childGroup]))
                        .GetHashCode()
                        .ToString();

                    if (!groupedContentData.ContainsKey(hash))
                    {
                        groupedContentData[hash] = Tuple.Create(report.HeaderColumns.ToDictionary(
                                column => column, column => row[column]),
                                new List<Dictionary<string, string>>());
                    }

                    groupedContentData[hash].Item2.Add(report.ContentColumns.ToDictionary(
                        column => column, column => row[column]));
                }

                contentData[group.Key] = groupedContentData;
            }

            return contentData;
        }
    }
}
