using System.ComponentModel.DataAnnotations.Schema;

namespace GestionProyectos.Data.Entities.Security
{
    [Table("User", Schema = "Security")]

    public class User : AuditBaseEntity
    {
        [Column("Name")]
        public string Name { get; set; } = string.Empty;
        [Column("LastName")]
        public string LastName { get; set; } = string.Empty;
        [Column("Email")]
        public string Email { get; set; } = string.Empty;
        [Column("Password")]
        public string Password { get; set; } = string.Empty;
        [Column("JobTitle")]
        public string JobTitle { get; set; } = string.Empty;
        [Column("AvatarFileName")]
        public string? AvatarFileName { get; set; }
        [Column("AvatarFilePath")]
        public string? AvatarFilePath { get; set; }
        public virtual List<UserRole> UserRole { get; set; } = new();
        public virtual List<UserTimeOff> TimeOffs { get; set; } = new();
    }
}
