using GestionProyectos.Engine.Feature.Project.Create.Request;
using GestionProyectos.Engine.Feature.Project.Create.Response;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Feature.Project.Create
{
    public interface IProjectCreateEngine
    {
        OperationResult<ProjectCreateResponse> Execute(ProjectCreateRequest request);
    }
}
