using System.Globalization;
using GestionProyectos.Data;
using GestionProyectos.Engine.Feature.HoursReport.Request;
using GestionProyectos.Engine.Feature.HoursReport.Response;
using GestionProyectos.Engine.Security.Utilities;
using GestionProyectos.Shared.Enums;
using GestionProyectos.Shared.Message;
using Microsoft.EntityFrameworkCore;

namespace GestionProyectos.Engine.Feature.HoursReport;

public class HoursReportEngine : IHoursReportEngine
{
    private static readonly CultureInfo EsCulture = CultureInfo.GetCultureInfo("es-ES");

    private readonly DataDbContext dbContext;

    public HoursReportEngine(DataDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public OperationResult<HoursReportReadResponse> Execute(HoursReportReadRequest request)
    {
        try
        {
            if (request.Year < 2000 || request.Year > 2100 || request.Month is < 1 or > 12)
                return OperationResult<HoursReportReadResponse>.CreateFailureResult("Mes o año no válido.");

            var startDate = new DateTime(request.Year, request.Month, 1);
            var endDate = startDate.AddMonths(1).AddTicks(-1);
            var daysInMonth = DateTime.DaysInMonth(request.Year, request.Month);
            var today = DateTime.Today;

            var days = Enumerable.Range(1, daysInMonth)
                .Select(day => new DateTime(request.Year, request.Month, day))
                .Where(date => date.Date <= today)
                .Select(date => new HoursReportDayColumn
                {
                    Date = DateTime.SpecifyKind(date, DateTimeKind.Unspecified),
                    Day = date.Day,
                    WeekDayShort = EsCulture.DateTimeFormat.GetAbbreviatedDayName(date.DayOfWeek),
                    IsWeekend = date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday
                })
                .ToList();

            var usersQuery = dbContext.User
                .AsNoTracking()
                .Where(u => u.RowStatus == (short)RowStatus.Active);

            if (IsDeveloperRole(request.Context))
                usersQuery = usersQuery.Where(u => u.Id == request.Context.UserId);

            var users = usersQuery
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.Name)
                .Select(u => new HoursReportUserRow
                {
                    UserId = u.Id,
                    UserName = (u.LastName + " " + u.Name).Trim()
                })
                .ToList();

            var logsQuery = dbContext.TimeLog
                .AsNoTracking()
                .Where(tl => tl.RowStatus == (short)RowStatus.Active)
                .Where(tl => tl.UsedHours > 0)
                .Where(tl => tl.ExecutionDate >= startDate && tl.ExecutionDate <= endDate);

            if (IsDeveloperRole(request.Context))
                logsQuery = logsQuery.Where(tl => tl.UserId == request.Context.UserId);

            // El truncado por día se hace en memoria: en SQL se resolvería con la zona horaria
            // del servidor y la fecha resultante no coincide con las columnas del mes.
            var loggedDays = logsQuery
                .Select(tl => new { tl.UserId, tl.ExecutionDate })
                .AsEnumerable()
                .Select(x => (x.UserId, Date: DateTime.SpecifyKind(x.ExecutionDate.Date, DateTimeKind.Unspecified)))
                .ToHashSet();

            var timeOffsQuery = dbContext.UserTimeOff
                .AsNoTracking()
                .Where(t => t.RowStatus == (short)RowStatus.Active)
                .Where(t => t.StartDate <= endDate && t.EndDate >= startDate);

            if (IsDeveloperRole(request.Context))
                timeOffsQuery = timeOffsQuery.Where(t => t.UserId == request.Context.UserId);

            var timeOffs = timeOffsQuery.ToList();
            var absentDays = new HashSet<(long UserId, DateTime Date)>();
            var absentDayTypes = new Dictionary<(long UserId, DateTime Date), short>();

            foreach (var timeOff in timeOffs)
            {
                var rangeStart = timeOff.StartDate.Date > startDate.Date ? timeOff.StartDate.Date : startDate.Date;
                var rangeEnd = timeOff.EndDate.Date < endDate.Date ? timeOff.EndDate.Date : endDate.Date;
                if (rangeEnd < rangeStart)
                    continue;

                for (var day = rangeStart; day <= rangeEnd && day <= today; day = day.AddDays(1))
                {
                    var key = (timeOff.UserId, DateTime.SpecifyKind(day, DateTimeKind.Unspecified));
                    absentDays.Add(key);
                    absentDayTypes[key] = timeOff.Type;
                }
            }

            return OperationResult<HoursReportReadResponse>.CreateSuccessResult(new HoursReportReadResponse
            {
                Year = request.Year,
                Month = request.Month,
                Days = days,
                Users = users,
                LoggedDays = loggedDays,
                AbsentDays = absentDays,
                AbsentDayTypes = absentDayTypes
            });
        }
        catch (Exception ex)
        {
            return OperationResult<HoursReportReadResponse>.CreateFailureResult(ex);
        }
    }

    private static bool IsDeveloperRole(Context context) =>
        context.Role?.Equals("Desarrollador", StringComparison.OrdinalIgnoreCase) == true;
}
