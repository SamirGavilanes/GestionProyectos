using GestionProyectos.Engine.Security.Utilities;

namespace GestionProyectos.Engine.Feature.Task.TimeLogRegistration.Request;

public class TimeLogRegistrationRequest
{
    public long TaskId { get; set; }
    public DateTime ExecutionDate { get; set; }
    public decimal UsedHours { get; set; }
    public decimal ProgressPercent { get; set; }
    public long HourTypeId { get; set; }
    public Context Context { get; set; } = null!;
}
