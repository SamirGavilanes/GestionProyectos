using GestionProyectos.Engine.Security.Utilities;

namespace GestionProyectos.Engine.Feature.Backlog.Request
{
    public class BacklogListRequest
    {
        public Context Context { get; set; } = new();
    }

    public class BacklogSaveRequest
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public long BacklogStatusId { get; set; }
        public long CustomerId { get; set; }
        public Context Context { get; set; } = new();
    }

    public class BacklogDeleteRequest
    {
        public long Id { get; set; }
        public Context Context { get; set; } = new();
    }
}
