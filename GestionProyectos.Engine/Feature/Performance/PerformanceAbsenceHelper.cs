using GestionProyectos.Data.Entities.Security;

namespace GestionProyectos.Engine.Feature.Performance;

public static class PerformanceAbsenceHelper
{
    public static int CountAbsentDays(IEnumerable<UserTimeOff> timeOffs, DateTime rangeStart, DateTime rangeEnd)
    {
        if (rangeEnd.Date < rangeStart.Date)
            return 0;

        var absentDays = new HashSet<DateTime>();
        foreach (var timeOff in timeOffs)
        {
            var start = timeOff.StartDate.Date > rangeStart.Date ? timeOff.StartDate.Date : rangeStart.Date;
            var end = timeOff.EndDate.Date < rangeEnd.Date ? timeOff.EndDate.Date : rangeEnd.Date;
            if (end < start)
                continue;

            for (var day = start; day <= end; day = day.AddDays(1))
                absentDays.Add(day);
        }

        return absentDays.Count;
    }

    public static decimal GetAvailabilityRatio(int calendarDays, int absentDays)
    {
        if (calendarDays <= 0)
            return 1m;

        var workingDays = calendarDays - absentDays;
        if (workingDays <= 0)
            return 0.01m;

        return Math.Max(0.01m, (decimal)workingDays / calendarDays);
    }

    public static (DateTime Start, DateTime End) GetTaskWindow(
        Data.Entities.TaskManagement.Task task,
        DateTime periodStart,
        DateTime periodEnd,
        bool useDateRange)
    {
        var taskStart = task.StartDate.Date;
        var taskEnd = (task.ActualEndDate ?? task.EndDate ?? DateTime.Today).Date;

        if (!useDateRange)
            return (taskStart, taskEnd);

        var start = taskStart > periodStart.Date ? taskStart : periodStart.Date;
        var end = taskEnd < periodEnd.Date ? taskEnd : periodEnd.Date;
        if (end < start)
            end = start;

        return (start, end);
    }
}
