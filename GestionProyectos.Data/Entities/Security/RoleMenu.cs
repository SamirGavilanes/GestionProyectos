using System.ComponentModel.DataAnnotations.Schema;

namespace GestionProyectos.Data.Entities.Security
{
    [Table("RoleMenu", Schema = "Security")]
    public class RoleMenu : AuditBaseEntity
    {
        [Column("RoleId")]
        public long RoleId { get; set; }
        [Column("MenuId")]
        public long MenuId { get; set; }
        [Column("CanView")]
        public bool CanView { get; set; } = true;
        [Column("CanCreate")]
        public bool CanCreate { get; set; } = true;
        [Column("CanEdit")]
        public bool CanEdit { get; set; } = true;
        [Column("CanDelete")]
        public bool CanDelete { get; set; } = true;
        [Column("CanRegisterHours")]
        public bool CanRegisterHours { get; set; } = true;
        [Column("CanFinalize")]
        public bool CanFinalize { get; set; }
        public virtual Menu Menu { get; set; } = null!;
    }
}
