namespace GestionProyectos.Engine.Feature.Task.TaskList.Response
{
    public class TaskListResponse
    {
        public List<TaskItem> Tasks { get; set; }
        public TaskListResponse()
        {
            Tasks = new();
        }
    }

    public class TaskItem
    {
        public long Id { get; set; }
        public long TicketId { get; set; }
        public long ProjectId { get; set; }
        public string ProjectDescription { get; set; }
        public string TaskDescription { get; set; }
        public string TicketDescription { get; set; }
        public decimal TimeEstimationHours { get; set; }
        public long TaskStatusId { get; set; }
        public string TaskStatus { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public decimal HoursWorkedInPeriod { get; set; }
        public bool IsWithinOriginalScope { get; set; } = true;
        public short? ScopeChangeReason { get; set; }
        public long DevelopmentPhaseId { get; set; }
        public string DevelopmentPhase { get; set; } = string.Empty;
        public decimal TotalHoursWorked { get; set; }
        public int BugCount { get; set; }
        public int ProgressPercent { get; set; }
        public DateTime? LastExecutionDate { get; set; }

        public TaskItem()
        {
            TaskDescription = string.Empty;
            TicketDescription = string.Empty;
            ProjectDescription = string.Empty;
            TaskStatus = string.Empty;
            UserName = string.Empty;
            DevelopmentPhase = string.Empty;
        }
    }
}
