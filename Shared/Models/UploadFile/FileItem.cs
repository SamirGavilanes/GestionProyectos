namespace GestionProyectos.Shared.Models.UploadFile
{
    public class FileItem
    {
        public string Name { get; set; } = string.Empty;
        public Stream File { get; set; } = null!;
    }
}
