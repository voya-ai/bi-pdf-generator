using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VoyaReporting.Models;
using VoyaReporting.Models.Domain;

namespace VoyaReporting.Services
{
    public class DomoCSVParser
    {
        public List<Dictionary<string, string>> Parse(string cachableResponse, Report report, Dictionary<string, int> header, GenerationSettings settings)
        {
            var data = new List<Dictionary<string, string>>();
            var allColumns = new HashSet<string>(report.HeaderColumns.Concat(report.ContentColumns));

            using (var responseReader = new StringReader(cachableResponse))
            {
                var reader = new CsvReader(responseReader, new CsvConfiguration()
                {
                    HasHeaderRecord = false
                });

                while (reader.Read())
                {
                    // skip data outside of bounds
                    if (settings.Timespan != 0 && header.ContainsKey(settings.TimeColumn))
                    {
                        var recordTime = DateTime.Now;

                        if (!DateTime.TryParse(reader.GetField<string>(header[settings.TimeColumn]).Trim(), out recordTime))
                        {
                            continue;
                        }

                        var startDate = DateTime.Parse(settings.StartDate);

                        if (recordTime < startDate || recordTime > startDate.AddDays(settings.Timespan))
                        {
                            continue;
                        }
                    }

                    var row = allColumns.ToDictionary(column => column,
                        column => reader.GetField<string>(header[column]).Trim());
                    data.Add(row);
                }
            }

            return data;
        }
    }
}
