using GestionProyectos.Engine.Feature.Task.TaskUpdate.Request;
using GestionProyectos.Engine.Feature.Task.TaskUpdate.Response;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Feature.Task.TaskUpdate
{
    public interface ITaskUpdateEngine
    {
        OperationResult<TaskUpdateResponse> Execute(TaskUpdateRequest request);
    }
}
