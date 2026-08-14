using GestionProyectos.Data;
using GestionProyectos.Engine.Excel.Download;
using GestionProyectos.Engine.Excel.Download.Request;
using GestionProyectos.Engine.Excel.Download.Response;
using GestionProyectos.Engine.Feature.Billing.Request;
using GestionProyectos.Engine.Feature.Billing.Response;
using GestionProyectos.Engine.Security.Utilities;
using GestionProyectos.Shared.Enums;
using GestionProyectos.Shared.Message;
using GestionProyectos.Shared.Models.Report;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Table;

namespace GestionProyectos.Engine.Feature.Billing;

public class BillingReportEngine : IBillingReportEngine
{
    private readonly DataDbContext dbContext;
    private readonly IExcelDownloadEngine excelDownloadEngine;

    public BillingReportEngine(DataDbContext dbContext, IExcelDownloadEngine excelDownloadEngine)
    {
        this.dbContext = dbContext;
        this.excelDownloadEngine = excelDownloadEngine;
    }

    public OperationResult<BillingReportResponse> Execute(BillingReportRequest request)
    {
        try
        {
            var startDate = request.StartDate.Date;
            var endDate = request.EndDate.Date.AddDays(1).AddTicks(-1);

            var hourTypes = dbContext.HourType
                .AsNoTracking()
                .Where(h => h.RowStatus == (short)RowStatus.Active)
                .OrderBy(h => h.Order)
                .ThenBy(h => h.Id)
                .Select(h => new BillingHourTypeColumn
                {
                    Id = h.Id,
                    Name = h.Description,
                    BadgeColor = h.BadgeColor
                })
                .ToList();

            var logsQuery = dbContext.TimeLog
                .AsNoTracking()
                .Include(tl => tl.User)
                .Include(tl => tl.HourType)
                .Include(tl => tl.Task)
                    .ThenInclude(t => t.Requirement)
                        .ThenInclude(r => r.Project)
                            .ThenInclude(p => p.Customer)
                                .ThenInclude(c => c.Enterprise)
                .Where(tl => tl.RowStatus == (short)RowStatus.Active)
                .Where(tl => tl.ExecutionDate >= startDate && tl.ExecutionDate <= endDate);

            if (IsDeveloperRole(request.Context))
                logsQuery = logsQuery.Where(tl => tl.UserId == request.Context.UserId);

            if (request.EnterpriseId > 0)
                logsQuery = logsQuery.Where(tl => tl.Task.Requirement.Project.Customer.EnterpriseId == request.EnterpriseId);

            if (request.CustomerId > 0)
                logsQuery = logsQuery.Where(tl => tl.Task.Requirement.Project.CustomerId == request.CustomerId);

            if (request.ProjectId > 0)
                logsQuery = logsQuery.Where(tl => tl.Task.Requirement.ProjectId == request.ProjectId);

            if (request.RequirementId > 0)
                logsQuery = logsQuery.Where(tl => tl.Task.RequirementId == request.RequirementId);

            if (request.HourTypeId > 0)
                logsQuery = logsQuery.Where(tl => tl.HourTypeId == request.HourTypeId);

            var logs = logsQuery
                .OrderBy(tl => tl.ExecutionDate)
                .ThenBy(tl => tl.Id)
                .ToList();

            var response = new BillingReportResponse();

            foreach (var log in logs)
            {
                var task = log.Task;
                var requirement = task.Requirement;
                var project = requirement.Project;
                var customer = project.Customer;

                response.Rows.Add(new BillingTimeLogRow
                {
                    TimeLogId = log.Id,
                    ExecutionDate = log.ExecutionDate,
                    UsedHours = log.UsedHours,
                    ProgressPercent = log.ProgressPercent,
                    HourTypeId = log.HourTypeId,
                    HourTypeName = log.HourType?.Description ?? string.Empty,
                    HourTypeBadgeColor = log.HourType?.BadgeColor ?? "gray",
                    TaskId = task.Id,
                    TaskDescription = task.Description,
                    RequirementId = requirement.Id,
                    RequirementDescription = requirement.Description,
                    ProjectId = project.Id,
                    ProjectDescription = project.Description,
                    CustomerName = customer.Description,
                    EnterpriseName = customer.Enterprise?.Description ?? string.Empty,
                    UserId = log.UserId,
                    UserName = log.User == null ? string.Empty : $"{log.User.Name} {log.User.LastName}".Trim()
                });
            }

            response.TotalHours = response.Rows.Sum(r => r.UsedHours);

            foreach (var hourType in hourTypes)
                hourType.TotalHours = response.Rows.Where(r => r.HourTypeId == hourType.Id).Sum(r => r.UsedHours);

            response.HourTypes = hourTypes;
            return OperationResult<BillingReportResponse>.CreateSuccessResult(response);
        }
        catch (Exception ex)
        {
            return OperationResult<BillingReportResponse>.CreateFailureResult(ex);
        }
    }

    public OperationResult<ExcelDownloadResponse> Export(BillingExportRequest request)
    {
        try
        {
            if (request.Data.Rows.Count == 0)
                return OperationResult<ExcelDownloadResponse>.CreateFailureResult("No hay registros para exportar.");

            if (request.Mode == BillingExportMode.Raw)
                return ExportRaw(request.Data);

            return ExportPivot(request);
        }
        catch (Exception ex)
        {
            return OperationResult<ExcelDownloadResponse>.CreateFailureResult(ex);
        }
    }

    private OperationResult<ExcelDownloadResponse> ExportRaw(BillingReportResponse data)
    {
        var rows = data.Rows.Select(r => (dynamic)new BillingExportRawRow
        {
            IdRegistro = r.TimeLogId,
            Fecha = r.ExecutionDate.ToString("dd/MM/yyyy"),
            Empresa = r.EnterpriseName,
            Cliente = r.CustomerName,
            Proyecto = r.ProjectDescription,
            IdRequerimiento = r.RequirementId,
            Requerimiento = r.RequirementDescription,
            IdTarea = r.TaskId,
            Tarea = r.TaskDescription,
            TipoHora = r.HourTypeName,
            Horas = r.UsedHours,
            AvancePct = r.ProgressPercent,
            MiembroDeEquipo = r.UserName
        }).ToList();

        var result = excelDownloadEngine.Download(new ExcelDownloadRequest
        {
            WorksheetName = "Horas",
            Rows = rows
        });

        if (result.Success && result.Data != null)
            result.Data.FileName = "facturacion-horas.xlsx";

        return result;
    }

    private OperationResult<ExcelDownloadResponse> ExportPivot(BillingExportRequest request)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Pivot");

        if (request.PivotGroupBy == BillingPivotGroupBy.HourType)
        {
            worksheet.Cells[1, 1].Value = "Tipo de hora";
            worksheet.Cells[1, 2].Value = "Horas";
            worksheet.Cells[1, 3].Value = "% del total";

            var rowIndex = 2;
            foreach (var hourType in request.Data.HourTypes.Where(h => h.TotalHours > 0))
            {
                worksheet.Cells[rowIndex, 1].Value = hourType.Name;
                worksheet.Cells[rowIndex, 2].Value = hourType.TotalHours;
                worksheet.Cells[rowIndex, 3].Value = request.Data.TotalHours > 0
                    ? Math.Round(hourType.TotalHours / request.Data.TotalHours * 100, 1)
                    : 0;
                rowIndex++;
            }

            worksheet.Cells[rowIndex, 1].Value = "Total general";
            worksheet.Cells[rowIndex, 2].Value = request.Data.TotalHours;
            worksheet.Cells[rowIndex, 3].Value = 100;
            rowIndex++;

            if (rowIndex > 2)
            {
                var table = worksheet.Tables.Add(worksheet.Cells[1, 1, rowIndex - 1, 3], "PivotData");
                table.ShowHeader = true;
                table.TableStyle = TableStyles.Medium2;
            }

            worksheet.Cells[1, 1, Math.Max(1, rowIndex - 1), 3].AutoFitColumns();
        }
        else
        {
            var hourTypes = request.Data.HourTypes;
            var pivotRows = BuildPivotRows(request.Data.Rows, hourTypes, request.PivotGroupBy);

            var headers = new List<string> { GetPivotGroupLabel(request.PivotGroupBy) };
            if (request.PivotGroupBy is BillingPivotGroupBy.Project or BillingPivotGroupBy.Requirement)
                headers.Add("Detalle");
            headers.AddRange(hourTypes.Select(h => h.Name));
            headers.Add("Total");

            for (var col = 0; col < headers.Count; col++)
                worksheet.Cells[1, col + 1].Value = headers[col];

            var rowIndex = 2;
            foreach (var row in pivotRows)
            {
                var colIndex = 1;
                worksheet.Cells[rowIndex, colIndex++].Value = row.Label;
                if (request.PivotGroupBy is BillingPivotGroupBy.Project or BillingPivotGroupBy.Requirement)
                    worksheet.Cells[rowIndex, colIndex++].Value = row.SubLabel;
                foreach (var hourType in hourTypes)
                    worksheet.Cells[rowIndex, colIndex++].Value = row.HoursByType.GetValueOrDefault(hourType.Id);
                worksheet.Cells[rowIndex, colIndex].Value = row.Total;
                rowIndex++;
            }

            if (pivotRows.Count > 0)
            {
                var totalRow = rowIndex;
                var col = 1;
                worksheet.Cells[totalRow, col++].Value = "Total general";
                if (request.PivotGroupBy is BillingPivotGroupBy.Project or BillingPivotGroupBy.Requirement)
                    worksheet.Cells[totalRow, col++].Value = string.Empty;
                foreach (var hourType in hourTypes)
                    worksheet.Cells[totalRow, col++].Value = pivotRows.Sum(r => r.HoursByType.GetValueOrDefault(hourType.Id));
                worksheet.Cells[totalRow, col].Value = pivotRows.Sum(r => r.Total);
                rowIndex++;
            }

            if (rowIndex > 2)
            {
                var table = worksheet.Tables.Add(
                    worksheet.Cells[1, 1, rowIndex - 1, headers.Count],
                    "PivotData");
                table.ShowHeader = true;
                table.TableStyle = TableStyles.Medium2;
            }

            worksheet.Cells[1, 1, Math.Max(1, rowIndex - 1), headers.Count].AutoFitColumns();
        }

        var response = new ExcelDownloadResponse
        {
            FileBase64 = Convert.ToBase64String(package.GetAsByteArray()),
            FileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            FileName = "facturacion-pivot.xlsx"
        };

        return OperationResult<ExcelDownloadResponse>.CreateSuccessResult(response);
    }

    public static List<BillingPivotRow> BuildPivotRows(
        IEnumerable<BillingTimeLogRow> rows,
        IReadOnlyList<BillingHourTypeColumn> hourTypes,
        BillingPivotGroupBy groupBy)
    {
        IEnumerable<IGrouping<string, BillingTimeLogRow>> groups = groupBy switch
        {
            BillingPivotGroupBy.Project => rows.GroupBy(r => $"{r.ProjectId}|{r.ProjectDescription}"),
            BillingPivotGroupBy.Requirement => rows.GroupBy(r => $"{r.RequirementId}|{r.RequirementDescription}|{r.ProjectDescription}"),
            BillingPivotGroupBy.Date => rows.GroupBy(r => r.ExecutionDate.ToString("yyyy-MM-dd")),
            BillingPivotGroupBy.HourType => rows.GroupBy(r => $"{r.HourTypeId}|{r.HourTypeName}"),
            _ => rows.GroupBy(r => $"{r.ProjectId}|{r.ProjectDescription}")
        };

        var pivotRows = new List<BillingPivotRow>();
        foreach (var group in groups.OrderBy(g => g.Key))
        {
            var pivot = new BillingPivotRow
            {
                HoursByType = hourTypes.ToDictionary(h => h.Id, _ => 0m)
            };

            switch (groupBy)
            {
                case BillingPivotGroupBy.Project:
                    var projectParts = group.Key.Split('|', 2);
                    pivot.Label = projectParts.Length > 1 ? projectParts[1] : group.Key;
                    pivot.SubLabel = group.First().CustomerName;
                    break;
                case BillingPivotGroupBy.Requirement:
                    var reqParts = group.Key.Split('|', 3);
                    pivot.Label = reqParts.Length > 1 ? $"#{reqParts[0]} · {reqParts[1]}" : group.Key;
                    pivot.SubLabel = reqParts.Length > 2 ? reqParts[2] : string.Empty;
                    break;
                case BillingPivotGroupBy.Date:
                    if (DateTime.TryParse(group.Key, out var date))
                        pivot.Label = date.ToString("dd/MM/yyyy");
                    else
                        pivot.Label = group.Key;
                    pivot.SubLabel = string.Empty;
                    break;
                case BillingPivotGroupBy.HourType:
                    var typeParts = group.Key.Split('|', 2);
                    pivot.Label = typeParts.Length > 1 ? typeParts[1] : group.Key;
                    pivot.SubLabel = string.Empty;
                    break;
            }

            foreach (var row in group)
            {
                if (pivot.HoursByType.ContainsKey(row.HourTypeId))
                    pivot.HoursByType[row.HourTypeId] += row.UsedHours;
                pivot.Total += row.UsedHours;
            }

            pivotRows.Add(pivot);
        }

        return pivotRows;
    }

    private static string GetPivotGroupLabel(BillingPivotGroupBy groupBy) => groupBy switch
    {
        BillingPivotGroupBy.Project => "Proyecto",
        BillingPivotGroupBy.Requirement => "Requerimiento",
        BillingPivotGroupBy.Date => "Fecha",
        BillingPivotGroupBy.HourType => "Tipo de hora",
        _ => "Grupo"
    };

    private static bool IsDeveloperRole(Context context) =>
        context.Role?.Equals("Desarrollador", StringComparison.OrdinalIgnoreCase) == true;
}

public class BillingPivotRow
{
    public string Label { get; set; } = string.Empty;
    public string SubLabel { get; set; } = string.Empty;
    public Dictionary<long, decimal> HoursByType { get; set; } = new();
    public decimal Total { get; set; }
}
