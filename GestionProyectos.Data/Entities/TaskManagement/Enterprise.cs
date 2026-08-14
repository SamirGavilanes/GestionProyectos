using System.ComponentModel.DataAnnotations.Schema;

namespace GestionProyectos.Data.Entities.TaskManagement
{
    [Table("Enterprise", Schema = "TaskManagement")]

    public class Enterprise : AuditBaseEntity, IDescribable
    {
        [Column("Description")]
        public string Description { get; set; } = string.Empty;

        public virtual List<Customer> Customers { get; set; } = new();
    }
}
