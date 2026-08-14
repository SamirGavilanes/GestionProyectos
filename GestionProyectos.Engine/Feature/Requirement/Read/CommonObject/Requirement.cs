namespace GestionProyectos.Engine.Feature.Requirement.Read.CommonObject

{

    public class Requirement

    {

        public long Id { get; set; }

        public string Description { get; set; } = string.Empty;

        public string Project { get; set; } = string.Empty;

        public string Scope { get; set; } = string.Empty;

        public long ProjectId { get; set; }

        public long RequirementStatusId { get; set; }

        public string RequirementStatus { get; set; } = string.Empty;

        public string RequirementStatusBadgeColor { get; set; } = "gray";

        public long PriorityId { get; set; }

        public string Priority { get; set; } = string.Empty;

        public string PriorityBadgeColor { get; set; } = "gray";

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

        public short RowStatus { get; set; }

        public List<FileAttachment> FileAttachments { get; set; } = new();

    }



    public class FileAttachment

    {

        public long Id { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;

    }

}


