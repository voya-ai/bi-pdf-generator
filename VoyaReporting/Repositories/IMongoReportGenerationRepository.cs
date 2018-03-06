using System.Collections.Generic;
using System.Threading.Tasks;
using VoyaReporting.Models.Domain;

namespace VoyaReporting.Repositories
{
    public interface IMongoReportGenerationRepository
    {
        Task AddAsync(ReportGeneration report);
        Task<List<ReportGeneration>> GetAsync();
        Task UpdateAsync(ReportGeneration report);
    }
}