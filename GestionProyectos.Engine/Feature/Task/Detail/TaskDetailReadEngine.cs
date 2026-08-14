using GestionProyectos.Data;
using GestionProyectos.Engine.Feature.Task.Detail.Request;
using GestionProyectos.Engine.Feature.Task.Detail.Response;
using GestionProyectos.Engine.Security.Utilities;
using GestionProyectos.Shared.Enums;
using GestionProyectos.Shared.Message;
using Microsoft.EntityFrameworkCore;

namespace GestionProyectos.Engine.Feature.Task.Detail;

public class TaskDetailReadEngine : ITaskDetailReadEngine
{
    private readonly DataDbContext dbContext;

    public TaskDetailReadEngine(DataDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public OperationResult<TaskDetailReadResponse> Execute(TaskDetailReadRequest request)
    {
        try
        {
            var task = dbContext.Task
                .Include(t => t.User)
                .Include(t => t.Priority)
                .Include(t => t.TaskStatus)
                .Include(t => t.DevelopmentPhase)
                .Include(t => t.Requirement)
                    .ThenInclude(r => r.RequirementStatus)
                .Include(t => t.Requirement)
                    .ThenInclude(r => r.Project)
                        .ThenInclude(p => p.Customer)
                .FirstOrDefault(t => t.Id == request.TaskId && t.RowStatus == (short)RowStatus.Active);

            if (task == null)
                return OperationResult<TaskDetailReadResponse>.CreateFailureResult("La tarea no existe.");

            if (IsDeveloperRole(request.Context) && task.UserId != request.Context.UserId)
                return OperationResult<TaskDetailReadResponse>.CreateFailureResult("No tiene acceso a esta tarea.");

            var statuses = dbContext.TaskStatus
                .Where(s => s.RowStatus == (short)RowStatus.Active)
                .ToDictionary(s => s.Id, s => s.Description);

            var totalHoursWorked = dbContext.TimeLog
                .Where(tl => tl.TaskId == task.Id && tl.RowStatus == (short)RowStatus.Active)
                .Sum(tl => (decimal?)tl.UsedHours) ?? 0;

            var bugCount = dbContext.TaskBug
                .Count(b => b.TaskId == task.Id && b.RowStatus == (short)RowStatus.Active);

            var noteCount = dbContext.TaskNote
                .Count(n => n.TaskId == task.Id && n.RowStatus == (short)RowStatus.Active);

            var response = new TaskDetailReadResponse
            {
                Task = new TaskDetailInfo
                {
                    Id = task.Id,
                    Description = task.Description,
                    RequirementId = task.RequirementId,
                    RequirementDescription = task.Requirement.Description,
                    RequirementStatus = task.Requirement.RequirementStatus?.Description ?? string.Empty,
                    RequirementStartDate = task.Requirement.StartDate,
                    RequirementEndDate = task.Requirement.EndDate,
                    ProjectId = task.Requirement.ProjectId,
                    ProjectDescription = task.Requirement.Project.Description,
                    CustomerDescription = task.Requirement.Project.Customer?.Description ?? string.Empty,
                    UserId = task.UserId ?? 0,
                    Responsible = $"{task.User?.Name} {task.User?.LastName}".Trim(),
                    PriorityId = task.PriorityId,
                    Priority = task.Priority.Description,
                    PriorityBadgeColor = task.Priority.BadgeColor,
                    TaskStatusId = task.TaskStatusId,
                    TaskStatus = task.TaskStatus.Description,
                    TaskStatusBadgeColor = task.TaskStatus.BadgeColor,
                    DevelopmentPhaseId = task.DevelopmentPhaseId,
                    DevelopmentPhase = task.DevelopmentPhase?.Description ?? string.Empty,
                    TimeEstimationHours = task.TimeEstimationHours,
                    TotalHoursWorked = totalHoursWorked,
                    StartDate = task.StartDate,
                    EndDate = task.EndDate,
                    ActualEndDate = task.ActualEndDate,
                    IsWithinOriginalScope = task.IsWithinOriginalScope,
                    ScopeChangeReason = task.ScopeChangeReason,
                    BugCount = bugCount,
                    NoteCount = noteCount
                }
            };

            var history = dbContext.TaskStatusHistory
                .Include(h => h.ChangedByUser)
                .Where(h => h.TaskId == task.Id && h.RowStatus == (short)RowStatus.Active)
                .OrderByDescending(h => h.Created)
                .ToList();

            foreach (var entry in history)
            {
                response.StatusHistory.Add(new TaskStatusHistoryItem
                {
                    Id = entry.Id,
                    Status = statuses.TryGetValue(entry.TaskStatusId, out var status) ? status : entry.TaskStatusId.ToString(),
                    PreviousStatus = entry.PreviousTaskStatusId.HasValue && statuses.TryGetValue(entry.PreviousTaskStatusId.Value, out var prev)
                        ? prev
                        : null,
                    Reason = entry.Reason ?? string.Empty,
                    ChangedBy = $"{entry.ChangedByUser.Name} {entry.ChangedByUser.LastName}".Trim(),
                    ChangedAt = entry.Created
                });
            }

            return OperationResult<TaskDetailReadResponse>.CreateSuccessResult(response);
        }
        catch (Exception ex)
        {
            return OperationResult<TaskDetailReadResponse>.CreateFailureResult(ex);
        }
    }

    private static bool IsDeveloperRole(Context context) =>
        context.Role?.Equals("Desarrollador", StringComparison.OrdinalIgnoreCase) == true;
}
