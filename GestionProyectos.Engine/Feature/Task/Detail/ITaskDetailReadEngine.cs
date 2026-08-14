using GestionProyectos.Engine.Feature.Task.Detail.Request;
using GestionProyectos.Engine.Feature.Task.Detail.Response;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Feature.Task.Detail;

public interface ITaskDetailReadEngine
{
    OperationResult<TaskDetailReadResponse> Execute(TaskDetailReadRequest request);
}
