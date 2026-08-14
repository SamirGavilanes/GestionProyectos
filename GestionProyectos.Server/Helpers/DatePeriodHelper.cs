using GestionProyectos.Server.Models;

namespace GestionProyectos.Server.Helpers;

public static class DatePeriodHelper
{
    public static readonly IReadOnlyList<(DatePeriodPreset Preset, string Label)> Options = new[]
    {
        (DatePeriodPreset.LastYear, "Último año"),
        (DatePeriodPreset.Last6Months, "Último 6 meses"),
        (DatePeriodPreset.Last3Months, "Último 3 meses"),
        (DatePeriodPreset.LastMonth, "Último mes"),
        (DatePeriodPreset.Custom, "Personalizado")
    };

    public static DatePeriodPreset DefaultPreset => DatePeriodPreset.LastMonth;

    public static (DateTime Start, DateTime End) GetRange(DatePeriodPreset preset, DateTime? referenceDate = null)
    {
        var today = (referenceDate ?? DateTime.Today).Date;

        return preset switch
        {
            DatePeriodPreset.LastYear => (today.AddYears(-1), today),
            DatePeriodPreset.Last6Months => (today.AddMonths(-6), today),
            DatePeriodPreset.Last3Months => (today.AddMonths(-3), today),
            DatePeriodPreset.LastMonth => (new DateTime(today.Year, today.Month, 1), today),
            _ => (new DateTime(today.Year, today.Month, 1), today)
        };
    }

    public static void ApplyPreset(DatePeriodPreset preset, out DateTime start, out DateTime end, DateTime? referenceDate = null)
    {
        (start, end) = GetRange(preset, referenceDate);
    }

    public static DatePeriodPreset DetectPreset(DateTime start, DateTime end, DateTime? referenceDate = null)
    {
        foreach (var option in Options)
        {
            if (option.Preset == DatePeriodPreset.Custom)
                continue;

            var (presetStart, presetEnd) = GetRange(option.Preset, referenceDate);
            if (start.Date == presetStart.Date && end.Date == presetEnd.Date)
                return option.Preset;
        }

        return DatePeriodPreset.Custom;
    }

    public static bool IsFilterActive(DatePeriodPreset preset, DateTime start, DateTime end, DateTime? referenceDate = null)
    {
        if (preset != DatePeriodPreset.Custom)
            return preset != DefaultPreset;

        var (defaultStart, defaultEnd) = GetRange(DefaultPreset, referenceDate);
        return start.Date != defaultStart.Date || end.Date != defaultEnd.Date;
    }

    public static IReadOnlyDictionary<DatePeriodPreset, int> CountByPresets(
        Func<DateTime, DateTime, int> countInRange,
        DateTime customStart,
        DateTime customEnd,
        DateTime? referenceDate = null)
    {
        var counts = new Dictionary<DatePeriodPreset, int>();
        foreach (var (preset, _) in Options)
        {
            if (preset == DatePeriodPreset.Custom)
            {
                counts[preset] = countInRange(customStart.Date, customEnd.Date);
                continue;
            }

            var (start, end) = GetRange(preset, referenceDate);
            counts[preset] = countInRange(start.Date, end.Date);
        }

        return counts;
    }

    public static bool IsDateInRange(DateTime date, DateTime start, DateTime end) =>
        date.Date >= start.Date && date.Date <= end.Date;

    public static bool RangesOverlap(DateTime rangeStart, DateTime rangeEnd, DateTime periodStart, DateTime periodEnd) =>
        rangeStart.Date <= periodEnd.Date && rangeEnd.Date >= periodStart.Date;
}
