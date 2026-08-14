using System.ComponentModel.DataAnnotations.Schema;

namespace GestionProyectos.Data.Entities.TaskManagement
{
    [Table("Project", Schema = "TaskManagement")]
    public class Project : AuditBaseEntity
    {
        [Column("Description")]
        public string Description { get; set; } = string.Empty;
        [Column("CustomerId")]
        public long CustomerId { get; set; }
        [Column("ProjectStatusId")]
        public long ProjectStatusId { get; set; }
        [Column("StartDate")]
        public DateTime StartDate { get; set; }
        [Column("EndDate")]
        public DateTime? EndDate { get; set; }
        [Column("ActualEndDate")]
        public DateTime? ActualEndDate { get; set; }
        public virtual Customer Customer { get; set; } = null!;
        public virtual ProjectStatus ProjectStatus { get; set; } = null!;
        public virtual List<Requirement> Requirements { get; set; } = new();
    }
}
