using System.ComponentModel.DataAnnotations.Schema;

namespace GestionProyectos.Data.Entities.TaskManagement
{
    [Table("Customer", Schema = "TaskManagement")]

    public class Customer : AuditBaseEntity, IDescribable
    {
        [Column("Description")]
        public string Description { get; set; } = string.Empty;
        [Column("EnterpriseId")]
        public long EnterpriseId { get; set; }
        public virtual Enterprise Enterprise { get; set; } = null!;
        public virtual List<Project> Projects { get; set; } = new();
    }
}
