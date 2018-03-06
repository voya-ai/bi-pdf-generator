using System.Collections.Generic;
using System.Threading.Tasks;
using VoyaReporting.Models.Domain;

namespace VoyaReporting.Repositories
{
    public interface IMongoReportRepository
    {
        Task AddAsync(Report report);
        Task UpdateAsync(Report report);
        Task<Report> GetAsync(string slug);
        Task<List<Report>> GetAsync();
    }
}