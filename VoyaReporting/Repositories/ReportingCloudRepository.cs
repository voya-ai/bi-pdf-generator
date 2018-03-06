using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using TXTextControl.ReportingCloud;
using VoyaReporting.Models;
using VoyaReporting.Models.Domain;

namespace VoyaReporting.Repositories
{
    public class ReportingCloudRepository : IReportingCloudRepository
    {
        private ReportingCloud connector;

        private bool IsTest;

        public ReportingCloudRepository()
        {
            var username = Environment.GetEnvironmentVariable("REPORTINGCLOUD_USERNAME");
            var password = Environment.GetEnvironmentVariable("REPORTINGCLOUD_PASSWORD");

            IsTest = bool.Parse(Environment.GetEnvironmentVariable("REPORTINGCLOUD_TEST") ?? bool.TrueString);

            connector = new ReportingCloud(username, password);
        }

        public List<byte[]> GenerateReport(Report report, JArray data)
        {
            if (!connector.TemplateExists(report.TemplateName))
            {
                throw new Exception($"Template {report.TemplateName} does not exist.");
            }

            var files = connector.MergeDocument(new MergeBody()
            {
                MergeData = data
            }, report.TemplateName,
            append: false,
            test: IsTest);

            return files;
        }

        public List<Template> Get(string name)
        {
            var templates = connector.ListTemplates();
            
            return templates
                .Where(template => string.IsNullOrEmpty(name)
                    || template.TemplateName.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();
        }
    }
}
