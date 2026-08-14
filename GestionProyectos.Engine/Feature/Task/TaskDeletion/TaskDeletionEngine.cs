using GestionProyectos.Data;
using GestionProyectos.Engine.Feature.Task.TaskDeletion.Request;
using GestionProyectos.Engine.Feature.Task.TaskDeletion.Response;
using GestionProyectos.Engine.Security.Utilities;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Feature.Task.TaskDeletion
{
    public class TaskDeletionEngine : ITaskDeletionEngine
    {
        private readonly DataDbContext dbContext;

        public TaskDeletionEngine(DataDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public OperationResult<TaskDeletionResponse> Execute(TaskDeletionRequest request)
        {
            try
            {
                TaskDeletionResponse response = new();
                var task = dbContext.Task.FirstOrDefault(x => x.Id == request.TaskId);

                if (task == null)
                    return OperationResult<TaskDeletionResponse>.CreateFailureResult("La tarea que quiere borrar ya no existe.");

                if (!PermissionHelper.CanDelete(request.Context, PermissionHelper.TaskManagementRoute))
                    return OperationResult<TaskDeletionResponse>.CreateFailureResult("Solo los administradores pueden eliminar tareas.");

                dbContext.Task.Remove(task);
                dbContext.SaveChanges();

                return OperationResult<TaskDeletionResponse>.CreateSuccessResult(response);

            }
            catch (Exception ex)
            {
                return OperationResult<TaskDeletionResponse>.CreateFailureResult(ex);
            }
        }
    }
}
