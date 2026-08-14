using GestionProyectos.Data.Entities.Security;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionProyectos.Data.Entities.TaskManagement
{
    [Table("TaskNote", Schema = "TaskManagement")]
    public class TaskNote : AuditBaseEntity
    {
        [Column("TaskId")]
        public long TaskId { get; set; }
        [Column("Content")]
        public string Content { get; set; } = string.Empty;
        [Column("AuthorUserId")]
        public long AuthorUserId { get; set; }

        public virtual Task Task { get; set; } = null!;
        public virtual User AuthorUser { get; set; } = null!;
    }
}
