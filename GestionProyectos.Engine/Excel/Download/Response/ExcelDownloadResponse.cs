namespace GestionProyectos.Engine.Excel.Download.Response
{
    public class ExcelDownloadResponse
    {
        public string FileType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FileBase64 { get; set; } = string.Empty;
    }
}
