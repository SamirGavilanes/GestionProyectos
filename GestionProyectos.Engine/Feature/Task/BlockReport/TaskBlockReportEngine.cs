using GestionProyectos.Data;
using GestionProyectos.Data.Entities.TaskManagement;
using GestionProyectos.Engine.Feature;
using GestionProyectos.Engine.Feature.Task.BlockReport.Request;
using GestionProyectos.Engine.Feature.Task.BlockReport.Response;
using GestionProyectos.Engine.Security.Utilities;
using GestionProyectos.Shared.Enums;
using GestionProyectos.Shared.Message;
using Microsoft.EntityFrameworkCore;

namespace GestionProyectos.Engine.Feature.Task.BlockReport
{
    public class TaskBlockReportEngine : ITaskBlockReportEngine
    {
        public const string PageRoute = "/bloqueos-externos";

        private readonly DataDbContext dbContext;

        public TaskBlockReportEngine(DataDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public OperationResult<TaskBlockReportListResponse> GetExternalBlocks(TaskBlockReportListRequest request)
        {
            try
            {
                if (!PermissionHelper.CanView(request.Context, PageRoute))
                    return OperationResult<TaskBlockReportListResponse>.CreateFailureResult("No tiene permiso para ver bloqueos externos.");

                var tasks = dbContext.Task
                    .AsNoTracking()
                    .Include(t => t.TaskStatus)
                    .Include(t => t.Requirement)
                    .Include(t => t.StatusHistory)
                    .Where(t => t.RowStatus == (short)RowStatus.Active)
                    .ToList()
                    .Where(t => TaskStatusHelper.IsExternalBlock(t.TaskStatus.Description))
                    .OrderByDescending(t => t.Updated ?? t.Created)
                    .ToList();

                var response = new TaskBlockReportListResponse();
                foreach (var task in tasks)
                {
                    response.Items.Add(new TaskBlockReportItem
                    {
                        TaskId = task.Id,
                        TaskDescription = task.Description,
                        RequirementId = task.RequirementId,
                        RequirementDescription = task.Requirement.Description,
                        TaskStatusId = task.TaskStatusId,
                        Status = task.TaskStatus.Description,
                        StatusBadgeColor = task.TaskStatus.BadgeColor,
                        Reason = ResolveReason(task)
                    });
                }

                return OperationResult<TaskBlockReportListResponse>.CreateSuccessResult(response);
            }
            catch (Exception ex)
            {
                return OperationResult<TaskBlockReportListResponse>.CreateFailureResult(ex);
            }
        }

        public OperationResult<bool> Update(TaskBlockReportUpdateRequest request)
        {
            try
            {
                if (!PermissionHelper.CanEdit(request.Context, PageRoute))
                    return OperationResult<bool>.CreateFailureResult("No tiene permiso para editar bloqueos externos.");

                var task = dbContext.Task
                    .Include(t => t.StatusHistory)
                    .FirstOrDefault(t => t.Id == request.TaskId && t.RowStatus == (short)RowStatus.Active);
                if (task == null)
                    return OperationResult<bool>.CreateFailureResult("No se encontró la tarea.");

                var newStatus = dbContext.TaskStatus.FirstOrDefault(s =>
                    s.Id == request.TaskStatusId && s.RowStatus == (short)RowStatus.Active);
                if (newStatus == null)
                    return OperationResult<bool>.CreateFailureResult("El estado no es válido.");

                if (FinalizeStatusHelper.IsTaskFinalized(newStatus.Description) &&
                    !PermissionHelper.CanFinalizeTask(request.Context))
                    return OperationResult<bool>.CreateFailureResult("No tiene permiso para finalizar tareas.");

                var reason = request.Reason?.Trim() ?? string.Empty;
                if (TaskStatusHelper.RequiresBlockReason(newStatus.Description) && string.IsNullOrWhiteSpace(reason))
                    return OperationResult<bool>.CreateFailureResult("Indique el motivo del bloqueo.");

                var previousStatusId = task.TaskStatusId;
                if (request.TaskStatusId != previousStatusId)
                {
                    task.TaskStatusId = request.TaskStatusId;
                    dbContext.TaskStatusHistory.Add(new TaskStatusHistory
                    {
                        TaskId = task.Id,
                        TaskStatusId = request.TaskStatusId,
                        PreviousTaskStatusId = previousStatusId,
                        Reason = reason,
                        ChangedByUserId = request.Context.UserId,
                        RowStatus = (short)RowStatus.Active,
                        Created = DateTime.UtcNow,
                        CreatedBy = request.Context.UserId
                    });
                }
                else
                {
                    var latest = task.StatusHistory
                        .Where(h => h.RowStatus == (short)RowStatus.Active && h.TaskStatusId == task.TaskStatusId)
                        .OrderByDescending(h => h.Created)
                        .ThenByDescending(h => h.Id)
                        .FirstOrDefault();

                    if (latest != null)
                    {
                        latest.Reason = reason;
                        latest.Updated = DateTime.UtcNow;
                        latest.UpdatedBy = request.Context.UserId;
                    }
                    else
                    {
                        dbContext.TaskStatusHistory.Add(new TaskStatusHistory
                        {
                            TaskId = task.Id,
                            TaskStatusId = task.TaskStatusId,
                            PreviousTaskStatusId = null,
                            Reason = reason,
                            ChangedByUserId = request.Context.UserId,
                            RowStatus = (short)RowStatus.Active,
                            Created = DateTime.UtcNow,
                            CreatedBy = request.Context.UserId
                        });
                    }
                }

                task.Updated = DateTime.UtcNow;
                task.UpdatedBy = request.Context.UserId;
                dbContext.SaveChanges();
                return OperationResult<bool>.CreateSuccessResult(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.CreateFailureResult(ex);
            }
        }

        private static string ResolveReason(Data.Entities.TaskManagement.Task task)
        {
            var latest = task.StatusHistory
                .Where(h => h.RowStatus == (short)RowStatus.Active && h.TaskStatusId == task.TaskStatusId)
                .OrderByDescending(h => h.Created)
                .ThenByDescending(h => h.Id)
                .FirstOrDefault();

            if (latest != null && !string.IsNullOrWhiteSpace(latest.Reason))
                return latest.Reason;

            return task.StatusHistory
                .Where(h => h.RowStatus == (short)RowStatus.Active && !string.IsNullOrWhiteSpace(h.Reason))
                .OrderByDescending(h => h.Created)
                .ThenByDescending(h => h.Id)
                .Select(h => h.Reason)
                .FirstOrDefault() ?? string.Empty;
        }
    }
}
