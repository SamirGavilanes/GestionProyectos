using System.ComponentModel.DataAnnotations.Schema;

namespace GestionProyectos.Data.Entities.TaskManagement
{
    [Table("Attachment", Schema = "TaskManagement")]

    public class Attachment : AuditBaseEntity
    {
        [Column("RequirementId")]
        public long RequirementId { get; set; }
        [Column("FileName")]
        public string FileName { get; set; } = string.Empty;
        [Column("FilePath")]
        public string FilePath { get; set; } = string.Empty;

        public virtual Requirement Requirement { get; set; } = null!;
    }
}
