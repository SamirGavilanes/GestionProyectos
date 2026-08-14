namespace GestionProyectos.Shared.Models.Report
{
    public class ReportTaskTimeLogItem
    {
        public long IdTarea { get; set; }
        public long IdTimeLog { get; set; }
        public string Fecha { get; set; } = string.Empty;
        public long IdRequerimiento { get; set; }
        public string Requerimiento { get; set; } = string.Empty;
        public string DescripcionTarea { get; set; } = string.Empty;
        public decimal HorasEstimadas { get; set; }
    }
}
