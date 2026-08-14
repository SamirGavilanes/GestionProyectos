namespace GestionProyectos.Engine.Feature.Project.Read.CommonObject
{
    public class Project
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public long ProjectStatusId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Customer { get; set; } = string.Empty;
        public string Enterprise { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string StatusBadgeColor { get; set; } = "gray";
        public bool IsClosedStatus { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public string Created { get; set; } = string.Empty;
    }
}
