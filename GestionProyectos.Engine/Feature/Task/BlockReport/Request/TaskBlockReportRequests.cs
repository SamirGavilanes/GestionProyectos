using GestionProyectos.Engine.Security.Utilities;

namespace GestionProyectos.Engine.Feature.Task.BlockReport.Request
{
    public class TaskBlockReportListRequest
    {
        public Context Context { get; set; } = new();
    }

    public class TaskBlockReportUpdateRequest
    {
        public long TaskId { get; set; }
        public long TaskStatusId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public Context Context { get; set; } = new();
    }
}
