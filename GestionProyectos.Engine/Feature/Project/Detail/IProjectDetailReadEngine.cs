using GestionProyectos.Engine.Feature.Project.Detail.Request;
using GestionProyectos.Engine.Feature.Project.Detail.Response;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Feature.Project.Detail;

public interface IProjectDetailReadEngine
{
    OperationResult<ProjectDetailReadResponse> Execute(ProjectDetailReadRequest request);
}
