namespace GestionProyectos.Engine.Feature.Task.BlockReport.Response
{
    public class TaskBlockReportListResponse
    {
        public List<TaskBlockReportItem> Items { get; set; } = new();
    }

    public class TaskBlockReportItem
    {
        public long TaskId { get; set; }
        public string TaskDescription { get; set; } = string.Empty;
        public long RequirementId { get; set; }
        public string RequirementDescription { get; set; } = string.Empty;
        public long TaskStatusId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string StatusBadgeColor { get; set; } = "gray";
        public string Reason { get; set; } = string.Empty;
    }
}
