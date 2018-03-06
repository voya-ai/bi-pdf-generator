using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using TXTextControl.ReportingCloud;
using VoyaReporting.Models.Domain;

namespace VoyaReporting.Repositories
{
    public interface IReportingCloudRepository
    {
        List<byte[]> GenerateReport(Report report, JArray data);
        List<Template> Get(string name);
    }
}