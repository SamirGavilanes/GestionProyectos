using System.ComponentModel.DataAnnotations.Schema;

namespace GestionProyectos.Data.Entities.TaskManagement
{
    [Table("TaskBugAttachment", Schema = "TaskManagement")]
    public class TaskBugAttachment : AuditBaseEntity
    {
        [Column("TaskBugId")]
        public long TaskBugId { get; set; }
        [Column("FileName")]
        public string FileName { get; set; } = string.Empty;
        [Column("FilePath")]
        public string FilePath { get; set; } = string.Empty;

        public virtual TaskBug TaskBug { get; set; } = null!;
    }
}
