using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using GestionProyectos.Engine.Utility.S3DownloadFile.Request;
using GestionProyectos.Engine.Utility.S3DownloadFile.Response;
using GestionProyectos.Shared.Message;
using System.IO;

namespace GestionProyectos.Engine.Utility.S3DownloadFile
{
    public class S3DownloadFileEngine : IS3DownloadFileEngine
    {
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast2;
        public S3DownloadFileEngine()
        {
            
        }
        public OperationResult<S3DownloadFileResponse> Execute(S3DownloadFileRequest request)
        {
            try
            {
                IAmazonS3 s3Client = new AmazonS3Client(request.AccessKey, request.SecretAccessKey, bucketRegion);

                GetObjectRequest s3Request = new()
                {
                    BucketName = request.BuketName,
                    Key = request.FilePath,
                };

                var file = s3Client.GetObjectAsync(s3Request).Result;

                S3DownloadFileResponse response = new();
                using (var responseStream = file.ResponseStream)
                {
                    using MemoryStream memoryStream = new();
                    responseStream.CopyTo(memoryStream);

                    byte[] byteArray = memoryStream.ToArray();
                    response.File = byteArray;
                }

                return OperationResult<S3DownloadFileResponse>.CreateSuccessResult(response);
            }
            catch (Exception ex)
            {
                return OperationResult<S3DownloadFileResponse>.CreateFailureResult(ex);
            }
        }
    }
}
