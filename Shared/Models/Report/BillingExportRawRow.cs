using System.ComponentModel;

namespace GestionProyectos.Shared.Models.Report;

public class BillingExportRawRow
{
    public long IdRegistro { get; set; }
    public string Fecha { get; set; } = string.Empty;
    public string Empresa { get; set; } = string.Empty;
    public string Cliente { get; set; } = string.Empty;
    public string Proyecto { get; set; } = string.Empty;
    public long IdRequerimiento { get; set; }
    public string Requerimiento { get; set; } = string.Empty;
    public long IdTarea { get; set; }
    public string Tarea { get; set; } = string.Empty;
    public string TipoHora { get; set; } = string.Empty;
    public decimal Horas { get; set; }
    public decimal AvancePct { get; set; }
    [DisplayName("Miembro de equipo")]
    public string MiembroDeEquipo { get; set; } = string.Empty;
}
