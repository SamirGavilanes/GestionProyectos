namespace GestionProyectos.Engine.Feature.HoursReport.Response;

public class HoursReportReadResponse
{
    public int Year { get; set; }
    public int Month { get; set; }
    public List<HoursReportDayColumn> Days { get; set; } = new();
    public List<HoursReportUserRow> Users { get; set; } = new();
    public HashSet<(long UserId, DateTime Date)> LoggedDays { get; set; } = new();
    public HashSet<(long UserId, DateTime Date)> AbsentDays { get; set; } = new();
    public Dictionary<(long UserId, DateTime Date), short> AbsentDayTypes { get; set; } = new();
}

public class HoursReportDayColumn
{
    public DateTime Date { get; set; }
    public int Day { get; set; }
    public string WeekDayShort { get; set; } = string.Empty;
    public bool IsWeekend { get; set; }
}

public class HoursReportUserRow
{
    public long UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
}
