namespace GestionProyectos.Engine.Excel.Download.Request
{
    public class ExcelDownloadRequest
    {
        public string WorksheetName { get; set; } = string.Empty;
        public List<dynamic> Rows { get; set; } = new();
    }
}
