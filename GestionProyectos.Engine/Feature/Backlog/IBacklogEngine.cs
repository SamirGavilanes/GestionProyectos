using GestionProyectos.Engine.Feature.Backlog.Request;
using GestionProyectos.Engine.Feature.Backlog.Response;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Feature.Backlog
{
    public interface IBacklogEngine
    {
        OperationResult<BacklogListResponse> GetItems(BacklogListRequest request);
        OperationResult<BacklogSaveResponse> Save(BacklogSaveRequest request);
        OperationResult<bool> Delete(BacklogDeleteRequest request);
    }
}
