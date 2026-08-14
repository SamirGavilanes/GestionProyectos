using System.ComponentModel.DataAnnotations.Schema;

namespace GestionProyectos.Data.Entities.TaskManagement
{
    [Table("ProjectStatus", Schema = "TaskManagement")]
    public class ProjectStatus : AuditBaseEntity, IDescribable, IOrderable
    {
        [Column("Description")]
        public string Description { get; set; } = string.Empty;
        [Column("Order")]
        public int Order { get; set; }
        [Column("BadgeColor")]
        public string BadgeColor { get; set; } = "gray";
        [Column("IsClosed")]
        public bool IsClosed { get; set; }

        public virtual List<Project> Projects { get; set; } = new();
    }
}
