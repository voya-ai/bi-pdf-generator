using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VoyaReporting.Models;
using VoyaReporting.Models.Domain;

namespace VoyaReporting.Services
{
    public interface IReportGenerationService
    {
        JArray ConvertToData(Dictionary<string,
                Tuple<
                    Dictionary<string, string>,
                    List<Dictionary<string, string>>>> documents);
        Task<string> GenerateAsync(Report report, GenerationSettings settings);
    }
}