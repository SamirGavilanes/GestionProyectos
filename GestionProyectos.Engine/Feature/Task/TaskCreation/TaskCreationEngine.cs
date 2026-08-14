using GestionProyectos.Data;
using GestionProyectos.Engine.Feature.Task;
using GestionProyectos.Engine.Feature.Task.TaskCreation.Request;

using GestionProyectos.Engine.Feature.Task.TaskCreation.Response;

using GestionProyectos.Shared.Enums;

using GestionProyectos.Shared.Message;



namespace GestionProyectos.Engine.Feature.Task.TaskCreation

{

    public class TaskCreationEngine : ITaskCreationEngine

    {

        private readonly DataDbContext dbContext;



        public TaskCreationEngine(DataDbContext dbContext)

        {

            this.dbContext = dbContext;

        }



        public OperationResult<TaskCreationResponse> Execute(TaskCreationRequest request)

        {

            try
            {
                if (string.IsNullOrWhiteSpace(request.Description))
                    return OperationResult<TaskCreationResponse>.CreateFailureResult("La descripción es obligatoria.");
                if (request.TicketId == 0)
                    return OperationResult<TaskCreationResponse>.CreateFailureResult("Debe seleccionar un requerimiento.");
                if (request.TimeEstimationHours <= 0)
                    return OperationResult<TaskCreationResponse>.CreateFailureResult("Las horas estimadas deben ser mayores a cero.");
                if (!request.IsWithinOriginalScope && !IsValidScopeChangeReason(request.ScopeChangeReason))
                    return OperationResult<TaskCreationResponse>.CreateFailureResult("Indique el motivo del cambio de alcance.");

                var defaultPriority = dbContext.Priority

                    .Where(p => p.RowStatus == (short)RowStatus.Active)

                    .OrderBy(p => p.Id)

                    .Select(p => p.Id)

                    .FirstOrDefault();



                var defaultStatus = dbContext.TaskStatus
                    .Where(s => s.RowStatus == (short)RowStatus.Active)
                    .OrderBy(s => s.Id)
                    .Select(s => s.Id)
                    .FirstOrDefault();

                var plannedPhaseId = dbContext.TaskDevelopmentPhase
                    .Where(p => p.RowStatus == (short)RowStatus.Active && p.Description == "Planificada")
                    .Select(p => p.Id)
                    .FirstOrDefault();

                if (plannedPhaseId <= 0)
                    return OperationResult<TaskCreationResponse>.CreateFailureResult("No hay fase 'Planificada' configurada.");

                if (request.UserId > 0 && !dbContext.User.Any(u => u.Id == request.UserId && u.RowStatus == (short)RowStatus.Active))
                    return OperationResult<TaskCreationResponse>.CreateFailureResult("El desarrollador seleccionado no es válido.");

                var phaseOrder = dbContext.TaskDevelopmentPhase
                    .Where(p => p.Id == plannedPhaseId && p.RowStatus == (short)RowStatus.Active)
                    .Select(p => p.Order)
                    .FirstOrDefault();

                var now = DateTime.UtcNow;

                Data.Entities.TaskManagement.Task newTask = new()

                {

                    Description = request.Description!,

                    UserId = request.UserId > 0 ? request.UserId : null,

                    RequirementId = request.TicketId,

                    TimeEstimationHours = request.TimeEstimationHours,

                    PriorityId = defaultPriority > 0 ? defaultPriority : 1,

                    TaskStatusId = defaultStatus > 0 ? defaultStatus : 1,

                    DevelopmentPhaseId = plannedPhaseId,

                    QaEnteredAt = TaskPhaseHelper.IsQaOrBeyond(phaseOrder) ? now : null,

                    StartDate = now,

                    IsWithinOriginalScope = request.IsWithinOriginalScope,
                    ScopeChangeReason = request.IsWithinOriginalScope ? null : request.ScopeChangeReason,

                    RowStatus = (short)RowStatus.Active,

                    Created = now,

                    CreatedBy = request.Context.UserId

                };



                dbContext.Task.Add(newTask);

                dbContext.SaveChanges();



                return OperationResult<TaskCreationResponse>.CreateSuccessResult(new TaskCreationResponse());

            }

            catch (Exception ex)

            {

                return OperationResult<TaskCreationResponse>.CreateFailureResult(ex);

            }

        }

        private static bool IsValidScopeChangeReason(short? reason) =>
            reason == (short)TaskScopeChangeReason.Internal || reason == (short)TaskScopeChangeReason.External;
    }
}

