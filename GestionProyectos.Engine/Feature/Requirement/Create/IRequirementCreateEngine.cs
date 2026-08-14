using GestionProyectos.Engine.Feature.Requirement.Create.Request;
using GestionProyectos.Engine.Feature.Requirement.Create.Response;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Feature.Requirement.Create
{
    public interface IRequirementCreateEngine
    {
        OperationResult<RequirementCreateResponse> Execute(RequirementCreateRequest request);
    }
}
