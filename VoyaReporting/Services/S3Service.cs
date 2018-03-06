using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace VoyaReporting.Services
{
    public class S3Service : IS3Service
    {
        private string bucketName;
        private AmazonS3Client client;

        public S3Service()
        {
            bucketName = Environment.GetEnvironmentVariable("AWSS3_BUCKETNAME");

            var awsAccessKeyId = Environment.GetEnvironmentVariable("AWSS3_ACCESSKEYID");
            var awsSecretAccessKey = Environment.GetEnvironmentVariable("AWSS3_SECRETACCESSKEY");

            client = new AmazonS3Client(awsAccessKeyId, awsSecretAccessKey, RegionEndpoint.EUCentral1);
        }

        public async Task<string> UploadAsync(string filePath, string key)
        {
            var request = new PutObjectRequest()
            {
                BucketName = bucketName,
                Key = key,
                FilePath = filePath,
                CannedACL = S3CannedACL.PublicRead
            };

            var response = await client.PutObjectAsync(request);

            return request.Key;
        }
    }
}
