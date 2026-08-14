using System.ComponentModel.DataAnnotations.Schema;

namespace GestionProyectos.Data.Entities.Security
{
    [Table("Menu", Schema = "Security")]
    public class Menu :AuditBaseEntity
    {
        [Column("Description")]
        public string Description { get; set; } = string.Empty;
        [Column("Icon")]
        public int Icon { get; set; }
        [Column("Page")]
        public string Page { get; set; } = string.Empty;
        [Column("Parent")]
        public long? Parent { get; set; }
        [Column("Order")]
        public int Order { get; set; }
    }
}
