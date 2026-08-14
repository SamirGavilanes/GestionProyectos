using GestionProyectos.Data;

using GestionProyectos.Data.Entities.TaskManagement;

using GestionProyectos.Engine.Feature.Task.TimeLogRegistration.Request;

using GestionProyectos.Engine.Feature.Task.TimeLogRegistration.Response;

using GestionProyectos.Engine.Security.Utilities;

using GestionProyectos.Shared.Enums;

using GestionProyectos.Shared.Message;



namespace GestionProyectos.Engine.Feature.Task.TimeLogRegistration

{

    public class TimeLogRegistrationEngine : ITimeLogRegistrationEngine

    {

        private readonly DataDbContext dbContext;



        public TimeLogRegistrationEngine(DataDbContext dbContext)

        {

            this.dbContext = dbContext;

        }



        public OperationResult<TimeLogRegistrationResponse> Execute(TimeLogRegistrationRequest request)

        {

            try

            {

                var task = dbContext.Task.FirstOrDefault(t => t.Id == request.TaskId && t.RowStatus == (short)RowStatus.Active);

                if (task == null)

                    return OperationResult<TimeLogRegistrationResponse>.CreateFailureResult("La tarea no existe.");



                if (!PermissionHelper.CanView(request.Context, PermissionHelper.TaskManagementRoute))

                    return OperationResult<TimeLogRegistrationResponse>.CreateFailureResult("No tiene permiso para acceder a tareas.");



                if (request.UsedHours <= 0)

                    return OperationResult<TimeLogRegistrationResponse>.CreateFailureResult("Las horas deben ser mayores a cero.");

                var hourType = dbContext.HourType.FirstOrDefault(h =>
                    h.Id == request.HourTypeId && h.RowStatus == (short)RowStatus.Active);

                if (hourType == null)
                    return OperationResult<TimeLogRegistrationResponse>.CreateFailureResult("Seleccione un tipo de hora válido.");

                if (request.ProgressPercent < 0)
                    return OperationResult<TimeLogRegistrationResponse>.CreateFailureResult(
                        "El avance del día no puede ser negativo.");

                var currentProgress = dbContext.TimeLog
                    .Where(tl => tl.TaskId == task.Id && tl.RowStatus == (short)RowStatus.Active)
                    .Sum(tl => tl.ProgressPercent);

                if (currentProgress + request.ProgressPercent > 100)
                    return OperationResult<TimeLogRegistrationResponse>.CreateFailureResult(
                        "El avance total no puede superar el 100 %.");



                var timeLog = new TimeLog

                {

                    TaskId = task.Id,

                    UserId = request.Context.UserId,

                    ExecutionDate = request.ExecutionDate.Date,

                    UsedHours = request.UsedHours,

                    ProgressPercent = request.ProgressPercent,

                    HourTypeId = request.HourTypeId,

                    RowStatus = (short)RowStatus.Active,

                    Created = DateTime.UtcNow,

                    CreatedBy = request.Context.UserId

                };



                dbContext.TimeLog.Add(timeLog);

                dbContext.SaveChanges();



                return OperationResult<TimeLogRegistrationResponse>.CreateSuccessResult(new TimeLogRegistrationResponse());

            }

            catch (Exception ex)

            {

                return OperationResult<TimeLogRegistrationResponse>.CreateFailureResult(ex);

            }

        }

    }

}
