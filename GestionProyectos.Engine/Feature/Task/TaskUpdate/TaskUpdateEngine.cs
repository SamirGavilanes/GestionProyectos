using GestionProyectos.Data;
using GestionProyectos.Data.Entities.TaskManagement;
using GestionProyectos.Engine.Feature.Task;
using GestionProyectos.Engine.Feature.Task.TaskUpdate.Request;
using GestionProyectos.Engine.Feature.Task.TaskUpdate.Response;
using GestionProyectos.Engine.Feature;
using GestionProyectos.Engine.Security.Utilities;
using GestionProyectos.Shared.Enums;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Feature.Task.TaskUpdate
{
    public class TaskUpdateEngine : ITaskUpdateEngine
    {
        private readonly DataDbContext dbContext;

        public TaskUpdateEngine(DataDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public OperationResult<TaskUpdateResponse> Execute(TaskUpdateRequest request)
        {
            try
            {
                var task = dbContext.Task.FirstOrDefault(t => t.Id == request.Id);
                if (task == null)
                    return OperationResult<TaskUpdateResponse>.CreateFailureResult("La tarea a actualizar no existe.");

                if (!PermissionHelper.CanEdit(request.Context, PermissionHelper.TaskManagementRoute))
                    return OperationResult<TaskUpdateResponse>.CreateFailureResult("Solo los administradores pueden modificar tareas.");

                if (!request.IsWithinOriginalScope && !IsValidScopeChangeReason(request.ScopeChangeReason))
                    return OperationResult<TaskUpdateResponse>.CreateFailureResult("Indique el motivo del cambio de alcance.");

                if (request.UserId is > 0 && !dbContext.User.Any(u => u.Id == request.UserId && u.RowStatus == (short)RowStatus.Active))
                    return OperationResult<TaskUpdateResponse>.CreateFailureResult("El desarrollador seleccionado no es válido.");

                var previousStatusId = task.TaskStatusId;
                var newStatusId = request.TaskStatusId > 0 ? request.TaskStatusId : task.TaskStatusId;

                if (newStatusId != previousStatusId)
                {
                    var newStatus = dbContext.TaskStatus.FirstOrDefault(s => s.Id == newStatusId);
                    if (newStatus == null)
                        return OperationResult<TaskUpdateResponse>.CreateFailureResult("El estado seleccionado no es válido.");

                    if (TaskStatusHelper.RequiresBlockReason(newStatus.Description) &&
                        string.IsNullOrWhiteSpace(request.StatusChangeReason))
                        return OperationResult<TaskUpdateResponse>.CreateFailureResult("Indique el motivo del bloqueo.");

                    if (FinalizeStatusHelper.IsTaskFinalized(newStatus.Description) &&
                        !PermissionHelper.CanFinalizeTask(request.Context))
                        return OperationResult<TaskUpdateResponse>.CreateFailureResult("No tiene permiso para finalizar tareas.");

                    task.TaskStatusId = newStatusId;
                    dbContext.TaskStatusHistory.Add(new TaskStatusHistory
                    {
                        TaskId = task.Id,
                        TaskStatusId = newStatusId,
                        PreviousTaskStatusId = previousStatusId,
                        Reason = request.StatusChangeReason?.Trim() ?? string.Empty,
                        ChangedByUserId = request.Context.UserId,
                        RowStatus = (short)RowStatus.Active,
                        Created = DateTime.UtcNow,
                        CreatedBy = request.Context.UserId
                    });
                }

                task.Description = request.Description;
                task.RequirementId = request.TicketId;
                task.TimeEstimationHours = request.TimeEstimationHours;
                if (request.UserId.HasValue)
                    task.UserId = request.UserId.Value > 0 ? request.UserId.Value : null;
                task.IsWithinOriginalScope = request.IsWithinOriginalScope;
                task.ScopeChangeReason = request.IsWithinOriginalScope ? null : request.ScopeChangeReason;

                if (request.DevelopmentPhaseId > 0 && request.DevelopmentPhaseId != task.DevelopmentPhaseId)
                {
                    var phases = dbContext.TaskDevelopmentPhase
                        .Where(p => p.RowStatus == (short)RowStatus.Active)
                        .ToDictionary(p => p.Id, p => p.Order);

                    if (!phases.ContainsKey(request.DevelopmentPhaseId))
                        return OperationResult<TaskUpdateResponse>.CreateFailureResult("La fase de desarrollo no es válida.");

                    var currentOrder = phases.GetValueOrDefault(task.DevelopmentPhaseId);
                    var newOrder = phases[request.DevelopmentPhaseId];

                    if (!TaskPhaseHelper.CanChangePhase(currentOrder, newOrder))
                        return OperationResult<TaskUpdateResponse>.CreateFailureResult(
                            "Una tarea no puede regresar a Desarrollo después de pasar a QA.");

                    if (TaskPhaseHelper.IsQaOrBeyond(newOrder) && !task.QaEnteredAt.HasValue)
                        task.QaEnteredAt = DateTime.UtcNow;

                    task.DevelopmentPhaseId = request.DevelopmentPhaseId;
                }

                task.Updated = DateTime.UtcNow;
                task.UpdatedBy = request.Context.UserId;

                dbContext.SaveChanges();
                return OperationResult<TaskUpdateResponse>.CreateSuccessResult(new TaskUpdateResponse());
            }
            catch (Exception ex)
            {
                return OperationResult<TaskUpdateResponse>.CreateFailureResult(ex);
            }
        }

        private static bool IsValidScopeChangeReason(short? reason) =>
            reason == (short)TaskScopeChangeReason.Internal || reason == (short)TaskScopeChangeReason.External;
    }
}
