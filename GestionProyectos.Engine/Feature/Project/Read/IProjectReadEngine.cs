using GestionProyectos.Engine.Feature.Project.Read.Request;
using GestionProyectos.Engine.Feature.Project.Read.Response;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Feature.Project.Read
{
    public interface IProjectReadEngine
    {
        OperationResult<ProjectReadResponse> GetProjects();
        OperationResult<ProjectReadResponse> GetProjectsByCustomer(ProjectReadRequest request);
        OperationResult<ProjectReadResponse> GetProjects(ProjectReadRequest request);
        OperationResult<ProjectReadResponse> GetProjectById(long projectId);
    }
}
