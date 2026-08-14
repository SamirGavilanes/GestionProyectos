using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionProyectos.Data.Entities
{
    public class AuditBaseEntity
    {
        [Key]
        [Column("Id")]
        public long Id { get; set; }
        [Column("RowStatus")]
        public short RowStatus { get; set; }
        [Column("CreatedBy")]
        public long CreatedBy { get; set; }
        [Column("Created")]
        public DateTime Created { get; set; }
        [Column("UpdatedBy")]
        public long? UpdatedBy { get; set; }
        [Column("Updated")]
        public DateTime? Updated { get; set; }
    }
}
