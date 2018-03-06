using System;
using System.Threading.Tasks;

namespace VoyaReporting.Services
{
    public interface IS3Service
    {
        Task<string> UploadAsync(string filePath, string key);
    }
}