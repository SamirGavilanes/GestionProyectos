using GestionProyectos.Engine.Feature.Project.Burndown.Response;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Feature.Project.Burndown;

public interface IProjectBurndownEngine
{
    OperationResult<ProjectBurndownResponse> GetBurndown(long projectId);
}
