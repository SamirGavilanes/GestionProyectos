using GestionProyectos.Data;
using GestionProyectos.Engine.Feature.Requirement.DownloadFile.Request;
using GestionProyectos.Engine.Feature.Requirement.DownloadFile.Response;
using GestionProyectos.Engine.Utility.S3DownloadFile;
using GestionProyectos.Engine.Utility.S3DownloadFile.Request;
using GestionProyectos.Shared.Configurations;
using GestionProyectos.Shared.Message;
using Microsoft.Extensions.Options;

namespace GestionProyectos.Engine.Feature.Requirement.DownloadFile
{
    public class DownloadFileEngine : IDownloadFileEngine
    {
        private readonly DataDbContext dbContext;
        private readonly IS3DownloadFileEngine s3DownloadFileEngine;
        private readonly IOptions<AppSettingsManagerBase> appSettings;
        public DownloadFileEngine(DataDbContext dbContext,
            IS3DownloadFileEngine s3DownloadFileEngine,
            IOptions<AppSettingsManagerBase> appSettings
            )
        {
            this.dbContext = dbContext;
            this.s3DownloadFileEngine = s3DownloadFileEngine;
            this.appSettings = appSettings;
        }
        public OperationResult<DownloadFileResponse> Execute(DownloadFileRequest request)
        {
            try
            {
                var files = dbContext.Attachment.Where(x => x.RequirementId == request.RequirementId).ToList();

                if (!files.Any())
                    return OperationResult<DownloadFileResponse>.CreateFailureResult("No existen archivos adjuntos");

                DownloadFileResponse response = new();
                foreach (var file in files)
                {
                    S3DownloadFileRequest s3DownloadFileRequest = new()
                    {
                        AccessKey = appSettings.Value.Configurations.S3Config.AccessKey,
                        BuketName = appSettings.Value.Configurations.S3Config.BuketName,
                        SecretAccessKey = appSettings.Value.Configurations.S3Config.SecretAccessKey,
                        Active = appSettings.Value.Configurations.S3Config.Active,
                        FilePath = $"{file.FilePath}{file.FileName}"
                    };
                    var fileResponse = s3DownloadFileEngine.Execute(s3DownloadFileRequest);
                    if (fileResponse.Data == null)
                        continue;

                    response.AttachmentFiles.Add(new AttachmentFile
                    {
                        Name = file.FileName,
                        File = fileResponse.Data.File
                    });
                }

                return OperationResult<DownloadFileResponse>.CreateSuccessResult(response);
            }
            catch (Exception ex)
            {
                return OperationResult<DownloadFileResponse>.CreateFailureResult(ex);
            }
        }
    }
}
