using System.ComponentModel.DataAnnotations.Schema;

namespace GestionProyectos.Data.Entities.TaskManagement
{
    [Table("HourType", Schema = "TaskManagement")]
    public class HourType : AuditBaseEntity, IDescribable, IOrderable, IColorable
    {
        [Column("Description")]
        public string Description { get; set; } = string.Empty;

        [Column("Scope")]
        public string Scope { get; set; } = string.Empty;

        [Column("Order")]
        public int Order { get; set; }

        [Column("BadgeColor")]
        public string BadgeColor { get; set; } = "gray";

        public virtual List<TimeLog> TimeLogs { get; set; } = new();
    }
}
