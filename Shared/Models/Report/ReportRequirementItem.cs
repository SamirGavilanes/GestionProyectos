namespace GestionProyectos.Shared.Models.Report
{
    public class ReportRequirementItem
    {
        public long Id { get; set; }
        public string Descripcion { get; set; }
        public string Empresa { get; set; }
        public string Cliente { get; set; }
        public string Proyecto { get; set; }
        public string Alcance { get; set; }
        public decimal HorasPlanificadas { get; set; }
        public decimal HorasTotales { get; set; }
        public decimal DesfaseHoras { get; set; }
        public string FechaInicio { get; set; }
        public string Asignado { get; set; }
        public string Estado { get; set; }
        public ReportRequirementItem()
        {
            Descripcion = string.Empty;
            Empresa = string.Empty;
            Cliente = string.Empty;
            Proyecto = string.Empty;
            Alcance = string.Empty;
            FechaInicio = string.Empty;
            Asignado = string.Empty;
            Estado = string.Empty;
        }
    }
}
