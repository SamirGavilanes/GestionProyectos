using GestionProyectos.Engine.Feature.Project.Burndown.Response;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Feature.Requirement.Burndown;

public interface IRequirementBurndownEngine
{
    OperationResult<ProjectBurndownResponse> GetBurndown(long requirementId);
}
