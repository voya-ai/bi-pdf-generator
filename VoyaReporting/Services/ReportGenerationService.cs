using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using VoyaReporting.Models;
using VoyaReporting.Models.Domain;
using VoyaReporting.Repositories;
using VoyaReporting.Util;

namespace VoyaReporting.Services
{
    public class ReportGenerationService : IReportGenerationService
    {
        private IDomoService domoService;
        private IS3Service s3Service;
        private IMongoReportGenerationRepository mongoReportGenerationRepository;
        private IReportingCloudRepository reportingCloudRepository;

        public ReportGenerationService(IDomoService domoService,
            IS3Service s3Service,
            IMongoReportGenerationRepository mongoReportGenerationRepository,
            IReportingCloudRepository reportingCloudRepository)
        {
            this.domoService = domoService;
            this.s3Service = s3Service;
            this.mongoReportGenerationRepository = mongoReportGenerationRepository;
            this.reportingCloudRepository = reportingCloudRepository;
        }

        public JArray ConvertToData(Dictionary<string,
                Tuple<
                    Dictionary<string, string>,
                    List<Dictionary<string, string>>>> documents)
        {
            var data = new JArray(documents.Select((subgroup) =>
            {
                var documentData = new JObject(JObject.FromObject(subgroup.Value.Item1));
                var documentLines = new JArray(subgroup.Value.Item2.Select(item => JObject.FromObject(item)));
                documentData.Add("lines", documentLines);
                return documentData;
            }));

            return data;
        }

        public async Task<string> GenerateAsync(Report report, GenerationSettings settings)
        {
            string key = $"{DateTime.Now.ToString("yyyy-MM-dd/yyyy-MM-dd_HH-mm-ss")}_{Guid.NewGuid()}.zip";

            await mongoReportGenerationRepository.AddAsync(new ReportGeneration()
            {
                Slug = report.Slug,
                Name = key,
                Status = "Pending"
            });

            try
            {
                var data = await domoService.GetDataAsync(report, settings);
                string url = string.Empty;

                // TODO factor this to another file to make this class testable
                var tempPath = Path.GetTempFileName();
                using (var tempFile = File.Create(tempPath))
                using (var zipArchive = new ZipArchive(tempFile, ZipArchiveMode.Create))
                {
                    foreach (var group in data)
                    {
                        var convertedData = ConvertToData(group.Value);
                        var documents = reportingCloudRepository.GenerateReport(report, convertedData);

                        // count on the order of items in the JArray and in the original group to be the same, to figure out filenames
                        // single sanity check: counts should be the same
                        var renameFiles = false;
                        var uniqueFilenamesSet = new HashSet<string>();
                        if(documents.Count == convertedData.Count)
                        {
                            renameFiles = true;
                        }

                        for(var i = 0; i < documents.Count; i++)
                        {
                            var document = documents[i];

                            string filename;
                            if(renameFiles)
                            {
                                var groupedValues = group
                                    .Value
                                    .Values
                                    .ElementAt(i)
                                    .Item1
                                    .Values;

                                filename = string.Join("_", groupedValues
                                    .Skip(groupedValues.Count - settings.ChildGroups.Count)
                                    .Select(value => Slugify.ToSlug(value)));

                                if(uniqueFilenamesSet.Contains(filename))
                                {
                                    filename = $"{filename}_{Guid.NewGuid().ToString()}";
                                } else
                                {
                                    uniqueFilenamesSet.Add(filename);
                                }
                            } else
                            {
                                filename = Guid.NewGuid().ToString();
                            }

                            var zipEntry = zipArchive.CreateEntry($"{group.Key}/{filename}.pdf");
                            using (var zipStream = zipEntry.Open())
                            using (var stream = new MemoryStream(document))
                                await stream.CopyToAsync(zipStream);
                        }
                    }
                }

                url = await s3Service.UploadAsync(tempPath, key);

                await mongoReportGenerationRepository.UpdateAsync(new ReportGeneration()
                {
                    Name = key,
                    Status = "Success"
                });

                return url;
            }
            catch (Exception)
            {
                await mongoReportGenerationRepository.UpdateAsync(new ReportGeneration()
                {
                    Name = key,
                    Status = "Failed"
                });

                throw;
            }
        }
    }
}
