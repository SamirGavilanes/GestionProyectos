using GestionProyectos.Engine.Security.Utilities;

using GestionProyectos.Shared.Models.UploadFile;



namespace GestionProyectos.Engine.Feature.Requirement.Create.Request

{

    public class RequirementCreateRequest

    {

        public long ProjectId { get; set; }

        public long RequirementStatusId { get; set; }

        public long PriorityId { get; set; }

        public string Description { get; set; } = string.Empty;

        public string Scope { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? ActualEndDate { get; set; }

        public string RequesterName { get; set; } = string.Empty;

        public DateTime RequestDate { get; set; }

        public string ImpactedSystems { get; set; } = string.Empty;

        public string? FreshDeskTicketNumber { get; set; }

        public bool IsWithinOriginalScope { get; set; } = true;

        public short? ScopeChangeReason { get; set; }

        public bool IsProductionReprocess { get; set; }

        public List<FileItem> Files { get; set; } = new();

        public Context Context { get; set; } = null!;

    }

}


