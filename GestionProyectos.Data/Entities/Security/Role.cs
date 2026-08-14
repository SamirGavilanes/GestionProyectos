using System.ComponentModel.DataAnnotations.Schema;

namespace GestionProyectos.Data.Entities.Security
{
    [Table("Role", Schema = "Security")]
    public class Role : AuditBaseEntity
    {
        [Column("Description")]
        public string Description { get; set; } = string.Empty;
        public virtual List<RoleMenu> RoleMenus { get; set; } = new();
    }
}
