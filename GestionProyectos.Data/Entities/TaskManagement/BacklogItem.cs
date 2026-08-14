using System.ComponentModel.DataAnnotations.Schema;

namespace GestionProyectos.Data.Entities.TaskManagement
{
    [Table("BacklogItem", Schema = "TaskManagement")]
    public class BacklogItem : AuditBaseEntity
    {
        [Column("Name")]
        public string Name { get; set; } = string.Empty;
        [Column("Description")]
        public string Description { get; set; } = string.Empty;
        [Column("BacklogStatusId")]
        public long BacklogStatusId { get; set; }
        [Column("CustomerId")]
        public long? CustomerId { get; set; }

        public virtual BacklogStatus BacklogStatus { get; set; } = null!;
        public virtual Customer? Customer { get; set; }
    }
}
