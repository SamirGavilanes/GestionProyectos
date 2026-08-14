namespace GestionProyectos.Shared.Models.Report
{
    public class ReportProjectItem
    {
        public long Id { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string Cliente { get; set; } = string.Empty;
        public string Empresa { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Fecha { get; set; } = string.Empty;
    }
}
