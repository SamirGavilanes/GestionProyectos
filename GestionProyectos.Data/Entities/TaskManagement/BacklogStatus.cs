using System.ComponentModel.DataAnnotations.Schema;

namespace GestionProyectos.Data.Entities.TaskManagement
{
    [Table("BacklogStatus", Schema = "TaskManagement")]
    public class BacklogStatus : AuditBaseEntity, IDescribable, IOrderable, IColorable
    {
        [Column("Description")]
        public string Description { get; set; } = string.Empty;
        [Column("Order")]
        public int Order { get; set; }
        [Column("BadgeColor")]
        public string BadgeColor { get; set; } = "gray";
        [Column("IsClosed")]
        public bool IsClosed { get; set; }

        public virtual List<BacklogItem> BacklogItems { get; set; } = new();
    }
}
