using GestionProyectos.Data.Entities.Security;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionProyectos.Data.Entities.TaskManagement
{
    [Table("TaskStatusHistory", Schema = "TaskManagement")]
    public class TaskStatusHistory : AuditBaseEntity
    {
        [Column("TaskId")]
        public long TaskId { get; set; }
        [Column("TaskStatusId")]
        public long TaskStatusId { get; set; }
        [Column("PreviousTaskStatusId")]
        public long? PreviousTaskStatusId { get; set; }
        [Column("Reason")]
        public string Reason { get; set; } = string.Empty;
        [Column("ChangedByUserId")]
        public long ChangedByUserId { get; set; }

        public virtual Task Task { get; set; } = null!;
        public virtual TaskStatus TaskStatus { get; set; } = null!;
        public virtual TaskStatus? PreviousTaskStatus { get; set; }
        public virtual User ChangedByUser { get; set; } = null!;
    }
}
