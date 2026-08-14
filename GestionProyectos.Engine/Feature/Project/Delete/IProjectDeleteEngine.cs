using GestionProyectos.Engine.Feature.Project.Delete.Request;
using GestionProyectos.Engine.Feature.Project.Delete.Response;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Feature.Project.Delete
{
    public interface IProjectDeleteEngine
    {
        OperationResult<ProjectDeleteResponse> Execute(ProjectDeleteRequest request);
    }
}
