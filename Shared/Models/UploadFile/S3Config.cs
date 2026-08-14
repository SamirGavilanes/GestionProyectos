namespace GestionProyectos.Shared.Models.UploadFile
{
    public class S3Config
    {
        public string AccessKey { get; set; } = string.Empty;
        public string SecretAccessKey { get; set; } = string.Empty;
        public string BuketName { get; set; } = string.Empty;
        public string DestinationPath { get; set; } = string.Empty;
        public bool Active { get; set; }
    }
}
