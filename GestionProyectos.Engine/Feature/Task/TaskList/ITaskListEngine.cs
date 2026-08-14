using GestionProyectos.Engine.Feature.Task.TaskList.Request;
using GestionProyectos.Engine.Feature.Task.TaskList.Response;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Feature.Task.TaskList
{
    public interface ITaskListEngine
    {
        OperationResult<TaskListResponse> Execute(TaskListRequest request);

    }
}
