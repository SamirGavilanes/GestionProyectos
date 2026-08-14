using System.ComponentModel.DataAnnotations.Schema;

namespace GestionProyectos.Data.Entities.TaskManagement
{
    [Table("Priority", Schema = "TaskManagement")]
    public class Priority : AuditBaseEntity, IDescribable
    {
        [Column("Description")]
        public string Description { get; set; } = string.Empty;
        [Column("BadgeColor")]
        public string BadgeColor { get; set; } = "gray";

        public virtual List<Requirement> Requirements { get; set; } = new();
        public virtual List<Task> Tasks { get; set; } = new();
    }
}
