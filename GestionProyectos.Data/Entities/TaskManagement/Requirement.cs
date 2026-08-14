using GestionProyectos.Data.Entities.Security;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionProyectos.Data.Entities.TaskManagement
{
    [Table("Requirement", Schema = "TaskManagement")]
    public class Requirement : AuditBaseEntity
    {
        [Column("Description")]
        public string Description { get; set; } = string.Empty;
        [Column("ProjectId")]
        public long ProjectId { get; set; }
        [Column("Scope")]
        public string Scope { get; set; } = string.Empty;
        [Column("RequirementStatusId")]
        public long RequirementStatusId { get; set; }
        [Column("PriorityId")]
        public long PriorityId { get; set; }
        [Column("StartDate")]
        public DateTime StartDate { get; set; }
        [Column("EndDate")]
        public DateTime? EndDate { get; set; }
        [Column("ActualEndDate")]
        public DateTime? ActualEndDate { get; set; }
        [Column("RequesterName")]
        public string RequesterName { get; set; } = string.Empty;
        [Column("RequestDate")]
        public DateTime RequestDate { get; set; }
        [Column("ImpactedSystems")]
        public string ImpactedSystems { get; set; } = string.Empty;
        [Column("FreshDeskTicketNumber")]
        public string? FreshDeskTicketNumber { get; set; }
        [Column("IsWithinOriginalScope")]
        public bool IsWithinOriginalScope { get; set; } = true;
        [Column("ScopeChangeReason")]
        public short? ScopeChangeReason { get; set; }
        [Column("IsProductionReprocess")]
        public bool IsProductionReprocess { get; set; }
        public virtual Project Project { get; set; } = null!;
        public virtual RequirementStatus RequirementStatus { get; set; } = null!;
        public virtual List<Attachment> Attachments { get; set; } = new();
        public virtual Priority Priority { get; set; } = null!;
        public virtual List<Task> Tasks { get; set; } = new();
        public virtual List<TaskBug> Bugs { get; set; } = new();
    }
}
