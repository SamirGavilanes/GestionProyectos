using System.ComponentModel.DataAnnotations.Schema;

namespace GestionProyectos.Data.Entities.TaskManagement
{
    [Table("TaskDevelopmentPhase", Schema = "TaskManagement")]
    public class TaskDevelopmentPhase : AuditBaseEntity, IDescribable, IOrderable, IColorable
    {
        [Column("Description")]
        public string Description { get; set; } = string.Empty;
        [Column("Order")]
        public int Order { get; set; }
        [Column("BadgeColor")]
        public string BadgeColor { get; set; } = "gray";

        public virtual List<Task> Tasks { get; set; } = new();
    }
}
