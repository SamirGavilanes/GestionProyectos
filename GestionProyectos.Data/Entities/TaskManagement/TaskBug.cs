using GestionProyectos.Data.Entities.Security;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionProyectos.Data.Entities.TaskManagement
{
    [Table("TaskBug", Schema = "TaskManagement")]
    public class TaskBug : AuditBaseEntity
    {
        [Column("RequirementId")]
        public long RequirementId { get; set; }
        [Column("TaskId")]
        public long? TaskId { get; set; }
        [Column("Description")]
        public string Description { get; set; } = string.Empty;
        [Column("TaskBugStatusId")]
        public long TaskBugStatusId { get; set; }
        [Column("ReportedByUserId")]
        public long ReportedByUserId { get; set; }
        [Column("StartDate")]
        public DateTime StartDate { get; set; }
        [Column("EndDate")]
        public DateTime EndDate { get; set; }

        public virtual Requirement Requirement { get; set; } = null!;
        public virtual Task? Task { get; set; }
        public virtual TaskBugStatus TaskBugStatus { get; set; } = null!;
        public virtual User ReportedByUser { get; set; } = null!;
        public virtual List<TaskBugAttachment> Attachments { get; set; } = new();
    }
}
