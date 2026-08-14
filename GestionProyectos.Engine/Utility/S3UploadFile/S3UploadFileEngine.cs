using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using GestionProyectos.Data;
using GestionProyectos.Engine.Utility.S3UploadFile.Request;
using GestionProyectos.Engine.Utility.S3UploadFile.Response;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Utility.S3UploadFile
{
    public class S3UploadFileEngine : IS3UploadFileEngine
    {
        private readonly DataDbContext dbContext;
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast2;
        public S3UploadFileEngine(DataDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public OperationResult<S3UploadFileResponse> Execute(S3UploadFileRequest request)
        {
            try
            {
                IAmazonS3 s3Client = new AmazonS3Client(request.AccessKey, request.SecretAccessKey, bucketRegion);
                PutObjectRequest putObjectRequest = new()
                {
                    InputStream = request.File,
                    BucketName = request.BuketName,
                    Key = $"{request.DestinationPath}{request.Name}",
                    ContentType = "text/plain"
                };
                S3UploadFileResponse response = new();
                s3Client.PutObjectAsync(putObjectRequest);

                return OperationResult<S3UploadFileResponse>.CreateSuccessResult(response);
            }
            catch (Exception ex)
            {
                return OperationResult<S3UploadFileResponse>.CreateFailureResult(ex);
            }
        }
    }
}
