using GestionProyectos.Data.Entities.Security;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionProyectos.Data.Entities.TaskManagement
{
    [Table("Task", Schema = "TaskManagement")]

    public class Task : AuditBaseEntity
    {
        [Column("RequirementId")]
        public long RequirementId { get; set; }
        [Column("UserId")]
        public long? UserId { get; set; }
        [Column("TimeEstimationHours")]
        public decimal TimeEstimationHours { get; set; }
        [Column("Description")]
        public string Description { get; set; } = string.Empty;
        [Column("PriorityId")]
        public long PriorityId { get; set; }
        [Column("TaskStatusId")]
        public long TaskStatusId { get; set; }
        [Column("DevelopmentPhaseId")]
        public long DevelopmentPhaseId { get; set; }
        [Column("StartDate")]
        public DateTime StartDate { get; set; }
        [Column("EndDate")]
        public DateTime? EndDate { get; set; }
        [Column("ActualEndDate")]
        public DateTime? ActualEndDate { get; set; }
        [Column("QaEnteredAt")]
        public DateTime? QaEnteredAt { get; set; }
        [Column("IsWithinOriginalScope")]
        public bool IsWithinOriginalScope { get; set; } = true;
        [Column("ScopeChangeReason")]
        public short? ScopeChangeReason { get; set; }
        public virtual User? User { get; set; }
        public virtual Requirement Requirement { get; set; } = null!;
        public virtual Priority Priority { get; set; } = null!;
        public virtual TaskStatus TaskStatus { get; set; } = null!;
        public virtual TaskDevelopmentPhase DevelopmentPhase { get; set; } = null!;

        public virtual List<TimeLog> TimeLogs { get; set; } = new List<TimeLog>();
        public virtual List<TaskStatusHistory> StatusHistory { get; set; } = new List<TaskStatusHistory>();
        public virtual List<TaskBug> Bugs { get; set; } = new List<TaskBug>();
        public virtual List<TaskNote> Notes { get; set; } = new List<TaskNote>();
    }
}
