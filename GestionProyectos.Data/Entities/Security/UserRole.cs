using System.ComponentModel.DataAnnotations.Schema;

namespace GestionProyectos.Data.Entities.Security
{
    [Table("UserRole", Schema = "Security")]
    public class UserRole:AuditBaseEntity
    {
        [Column("UserId")]
        public long UserId { get; set; }
        [Column("RoleId")]
        public long RoleId { get; set; }
        public virtual Role Role { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
