using GestionProyectos.Engine.Feature.Task.BlockReport.Request;
using GestionProyectos.Engine.Feature.Task.BlockReport.Response;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Feature.Task.BlockReport
{
    public interface ITaskBlockReportEngine
    {
        OperationResult<TaskBlockReportListResponse> GetExternalBlocks(TaskBlockReportListRequest request);
        OperationResult<bool> Update(TaskBlockReportUpdateRequest request);
    }
}
