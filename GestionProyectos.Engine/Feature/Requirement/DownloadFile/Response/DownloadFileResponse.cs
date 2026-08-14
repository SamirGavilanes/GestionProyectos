namespace GestionProyectos.Engine.Feature.Requirement.DownloadFile.Response
{
    public class DownloadFileResponse
    {
        public List<AttachmentFile> AttachmentFiles { get; set; } = new();
        public DownloadFileResponse()
        {
            AttachmentFiles = new();
        }
    }

    public class AttachmentFile
    {
        public string Name { get; set; } = string.Empty;
        public byte[] File { get; set; } = null!;
    }
}
