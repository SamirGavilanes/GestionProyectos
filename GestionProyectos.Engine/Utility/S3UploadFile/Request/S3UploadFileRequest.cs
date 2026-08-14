using GestionProyectos.Shared.Models.UploadFile;

namespace GestionProyectos.Engine.Utility.S3UploadFile.Request
{
    public class S3UploadFileRequest : S3Config
    {
        public string Name { get; set; } = string.Empty;
        public Stream File { get; set; } = null!;
    }
}
