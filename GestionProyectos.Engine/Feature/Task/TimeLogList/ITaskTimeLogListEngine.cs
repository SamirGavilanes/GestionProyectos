using GestionProyectos.Engine.Feature.Task.TimeLogList.Request;
using GestionProyectos.Engine.Feature.Task.TimeLogList.Response;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Feature.Task.TimeLogList;

public interface ITaskTimeLogListEngine
{
    OperationResult<TaskTimeLogListResponse> Execute(TaskTimeLogListRequest request);
}
