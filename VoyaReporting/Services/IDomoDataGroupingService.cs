using System.Collections.Generic;
using VoyaReporting.Models;
using VoyaReporting.Models.Domain;

namespace VoyaReporting.Services
{
    public interface IDomoDataGroupingService
    {
        GroupedDomoData Group(List<Dictionary<string, string>> data, Report report, GenerationSettings settings);
    }
}