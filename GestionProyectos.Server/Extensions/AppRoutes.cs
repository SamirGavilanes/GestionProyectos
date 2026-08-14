namespace GestionProyectos.Server.Extensions;

public static class AppRoutes
{
    public const string ControlRegistroHoras = "/control-registro-horas";
    public const string ReporteHoras = "/reporte-horas";
    public const string BloqueosExternos = "/bloqueos-externos";

    public static string ProjectDetail(long projectId) => $"/project-detail/{projectId}";
    public static string ProjectAnalysis(long projectId) => $"/project-analysis/{projectId}";
    public static string RequirementAnalysis(long requirementId) => $"/requirement-analysis/{requirementId}";
    public static string RequirementDetail(long requirementId) => $"/create-requirement/{requirementId}";
    public static string TaskDetail(long taskId) => $"/task-detail/{taskId}";
}
