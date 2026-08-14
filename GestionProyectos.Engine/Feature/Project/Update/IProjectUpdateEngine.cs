using GestionProyectos.Engine.Feature.Project.Update.Request;
using GestionProyectos.Engine.Feature.Project.Update.Response;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Feature.Project.Update
{
    public interface IProjectUpdateEngine
    {
        OperationResult<ProjectUpdateResponse> Execute(ProjectUpdateRequest request);
    }
}
