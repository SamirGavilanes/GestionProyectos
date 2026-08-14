using GestionProyectos.Engine.Feature.Task.Bug.Request;
using GestionProyectos.Engine.Feature.Task.Bug.Response;
using GestionProyectos.Engine.Security.Utilities;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Feature.Task.Bug;

public interface ITaskBugEngine
{
    OperationResult<TaskBugListResponse> GetBugs(TaskBugListRequest request);
    OperationResult<TaskBugGlobalListResponse> GetAllBugs(TaskBugGlobalListRequest request);
    OperationResult<TaskBugSaveResponse> SaveBug(TaskBugSaveRequest request);
    OperationResult<bool> AssignBugToTask(TaskBugAssignRequest request);
    OperationResult<TaskBugTaskOptionsResponse> GetTasksForRequirement(long requirementId, Context context);
    OperationResult<bool> DeleteBug(TaskBugDeleteRequest request);
    OperationResult<TaskBugDownloadResponse> DownloadAttachment(TaskBugDownloadRequest request);
}
