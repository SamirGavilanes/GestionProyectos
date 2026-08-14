using GestionProyectos.Shared.Models.UploadFile;

namespace GestionProyectos.Engine.Utility.S3DownloadFile.Request
{
    public class S3DownloadFileRequest : S3Config
    {
        public string FilePath { get; set; } = string.Empty;
    }
}
