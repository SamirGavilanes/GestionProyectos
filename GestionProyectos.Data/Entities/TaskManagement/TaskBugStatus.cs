using System.ComponentModel.DataAnnotations.Schema;

namespace GestionProyectos.Data.Entities.TaskManagement
{
    [Table("TaskBugStatus", Schema = "TaskManagement")]
    public class TaskBugStatus : AuditBaseEntity, IDescribable, IOrderable, IColorable
    {
        [Column("Description")]
        public string Description { get; set; } = string.Empty;
        [Column("Order")]
        public int Order { get; set; }
        [Column("BadgeColor")]
        public string BadgeColor { get; set; } = "gray";

        public virtual List<TaskBug> Bugs { get; set; } = new();
    }
}
