using GestionProyectos.Engine.Feature.Task.TaskDeletion.Request;
using GestionProyectos.Engine.Feature.Task.TaskDeletion.Response;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Feature.Task.TaskDeletion
{
    public interface ITaskDeletionEngine
    {
        OperationResult<TaskDeletionResponse> Execute(TaskDeletionRequest request);

    }
}
