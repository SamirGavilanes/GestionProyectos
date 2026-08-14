namespace GestionProyectos.Engine.Feature.Backlog.Response
{
    public class BacklogListResponse
    {
        public List<BacklogItemResponse> Items { get; set; } = new();
    }

    public class BacklogItemResponse
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public long BacklogStatusId { get; set; }
        public long CustomerId { get; set; }
        public long EnterpriseId { get; set; }
        public string Customer { get; set; } = string.Empty;
        public string Enterprise { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string StatusBadgeColor { get; set; } = "gray";
        public bool StatusIsClosed { get; set; }
    }

    public class BacklogSaveResponse
    {
        public long Id { get; set; }
    }
}
