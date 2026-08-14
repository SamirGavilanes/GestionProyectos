using GestionProyectos.Data;
using GestionProyectos.Engine.Feature.Task.TimeLogList.Request;
using GestionProyectos.Engine.Feature.Task.TimeLogList.Response;
using GestionProyectos.Engine.Security.Utilities;
using GestionProyectos.Shared.Enums;
using GestionProyectos.Shared.Message;
using Microsoft.EntityFrameworkCore;

namespace GestionProyectos.Engine.Feature.Task.TimeLogList;

public class TaskTimeLogListEngine : ITaskTimeLogListEngine
{
    private readonly DataDbContext dbContext;

    public TaskTimeLogListEngine(DataDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public OperationResult<TaskTimeLogListResponse> Execute(TaskTimeLogListRequest request)
    {
        try
        {
            var task = dbContext.Task
                .AsNoTracking()
                .FirstOrDefault(t => t.Id == request.TaskId && t.RowStatus == (short)RowStatus.Active);

            if (task == null)
                return OperationResult<TaskTimeLogListResponse>.CreateFailureResult("La tarea no existe.");

            var logs = dbContext.TimeLog
                .AsNoTracking()
                .Include(tl => tl.User)
                .Include(tl => tl.HourType)
                .Where(tl => tl.TaskId == request.TaskId && tl.RowStatus == (short)RowStatus.Active)
                .OrderBy(tl => tl.ExecutionDate)
                .ThenBy(tl => tl.Id)
                .ToList();

            var response = new TaskTimeLogListResponse
            {
                TaskId = task.Id,
                TaskDescription = task.Description,
                TimeEstimationHours = task.TimeEstimationHours,
                TotalHoursWorked = logs.Sum(l => l.UsedHours)
            };

            decimal cumulativeProgress = 0;
            foreach (var log in logs)
            {
                cumulativeProgress += log.ProgressPercent;
                response.Items.Add(new TaskTimeLogListItem
                {
                    Id = log.Id,
                    ExecutionDate = log.ExecutionDate,
                    UsedHours = log.UsedHours,
                    ProgressPercent = cumulativeProgress,
                    ProgressDelta = log.ProgressPercent,
                    UserName = log.User == null
                        ? string.Empty
                        : $"{log.User.Name} {log.User.LastName}".Trim(),
                    HourTypeId = log.HourTypeId,
                    HourTypeName = log.HourType?.Description ?? string.Empty,
                    HourTypeBadgeColor = log.HourType?.BadgeColor ?? "gray"
                });
            }

            response.CurrentProgressPercent = cumulativeProgress;

            return OperationResult<TaskTimeLogListResponse>.CreateSuccessResult(response);
        }
        catch (Exception ex)
        {
            return OperationResult<TaskTimeLogListResponse>.CreateFailureResult(ex);
        }
    }
}
