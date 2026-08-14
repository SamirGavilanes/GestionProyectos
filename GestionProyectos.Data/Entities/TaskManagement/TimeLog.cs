using GestionProyectos.Data.Entities.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionProyectos.Data.Entities.TaskManagement
{
    [Table("TimeLog", Schema = "TaskManagement")]
    public class TimeLog : AuditBaseEntity
    {
        [Column("UserId")]
        public long UserId { get; set; }
        [Column("ExecutionDate")]
        public DateTime ExecutionDate { get; set; }
        [Column("UsedHours")]
        public decimal UsedHours { get; set; }

        [Column("ProgressPercent")]
        public decimal ProgressPercent { get; set; }

        [Column("TaskId")]
        public long TaskId { get; set; }

        [Column("HourTypeId")]
        public long HourTypeId { get; set; }

        public virtual Task Task { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual HourType HourType { get; set; } = null!;
    }
}
