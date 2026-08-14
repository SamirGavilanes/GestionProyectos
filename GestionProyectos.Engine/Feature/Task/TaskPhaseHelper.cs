using GestionProyectos.Data.Entities.TaskManagement;

namespace GestionProyectos.Engine.Feature.Task;

public static class TaskPhaseHelper
{
    public const int QaPhaseOrder = 2;

    public static bool IsQaOrBeyond(int phaseOrder) => phaseOrder >= QaPhaseOrder;

    public static bool CanChangePhase(int currentPhaseOrder, int newPhaseOrder) =>
        !(currentPhaseOrder >= QaPhaseOrder && newPhaseOrder < QaPhaseOrder);

    public static decimal CalculateBugHours(IEnumerable<TimeLog> logs, DateTime? qaEnteredAt)
    {
        if (!qaEnteredAt.HasValue)
            return 0;

        var threshold = qaEnteredAt.Value.Date;
        return logs
            .Where(l => l.ExecutionDate.Date >= threshold)
            .Sum(l => l.UsedHours);
    }
}
