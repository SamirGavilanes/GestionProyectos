using GestionProyectos.Engine.Feature.Requirement.Update.Request;
using GestionProyectos.Engine.Feature.Requirement.Update.Response;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Feature.Requirement.Update
{
    public interface IRequirementUpdateEngine
    {
        OperationResult<RequirementUpdateResponse> Execute(RequirementUpdateRequest request);
    }
}
