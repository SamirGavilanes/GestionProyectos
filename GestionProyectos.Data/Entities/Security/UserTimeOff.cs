using System.ComponentModel.DataAnnotations.Schema;

namespace GestionProyectos.Data.Entities.Security
{
    [Table("UserTimeOff", Schema = "Security")]
    public class UserTimeOff : AuditBaseEntity
    {
        [Column("UserId")]
        public long UserId { get; set; }
        [Column("Type")]
        public short Type { get; set; }
        [Column("StartDate")]
        public DateTime StartDate { get; set; }
        [Column("EndDate")]
        public DateTime EndDate { get; set; }
        [Column("Hours")]
        public decimal Hours { get; set; }
        [Column("Description")]
        public string Description { get; set; } = string.Empty;

        public virtual User User { get; set; } = null!;
    }
}
