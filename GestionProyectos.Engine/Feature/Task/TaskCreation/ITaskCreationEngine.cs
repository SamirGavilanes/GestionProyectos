using GestionProyectos.Engine.Feature.Task.TaskCreation.Request;
using GestionProyectos.Engine.Feature.Task.TaskCreation.Response;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Feature.Task.TaskCreation
{
    public interface ITaskCreationEngine
    {
        OperationResult<TaskCreationResponse> Execute(TaskCreationRequest request);

    }
}
