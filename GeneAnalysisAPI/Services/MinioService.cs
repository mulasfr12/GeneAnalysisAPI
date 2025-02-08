using Minio.DataModel.Args;
using Minio;

namespace GeneAnalysisAPI.Services
{
    public class MinioService
    {
        private readonly IMinioClient _minioClient;
        private readonly string _bucketName;

        public MinioService(IConfiguration config)
        {
            _minioClient = new MinioClient()
               .WithEndpoint(config["MinIO:Endpoint"])
               .WithCredentials(config["MinIO:AccessKey"], config["MinIO:SecretKey"])
               .Build(); // Now it correctly returns IMinioClient

            _bucketName = config["MinIO:BucketName"];
        }

        public async Task EnsureBucketExistsAsync()
        {
            var beArgs = new BucketExistsArgs().WithBucket(_bucketName);
            bool found = await _minioClient.BucketExistsAsync(beArgs);
            if (!found)
            {
                var mbArgs = new MakeBucketArgs().WithBucket(_bucketName);
                await _minioClient.MakeBucketAsync(mbArgs);
            }
        }

        public async Task UploadFileAsync(Stream fileStream, string fileName)
        {
            await EnsureBucketExistsAsync();

            var putObjectArgs = new PutObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(fileName)
                .WithStreamData(fileStream)
                .WithObjectSize(fileStream.Length);

            await _minioClient.PutObjectAsync(putObjectArgs);
        }

        public async Task<Stream> GetFileAsync(string fileName)
        {
            var memoryStream = new MemoryStream();

            var getObjectArgs = new GetObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(fileName)
                .WithCallbackStream((stream) => stream.CopyTo(memoryStream));

            await _minioClient.GetObjectAsync(getObjectArgs);
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
