using GestionProyectos.Engine.Feature.Requirement.Read.Request;
using GestionProyectos.Engine.Feature.Requirement.Read.Response;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Feature.Requirement.Read
{
    public interface IRequirementReadEngine
    {
        OperationResult<RequirementReadResponse> GetTickets();
        OperationResult<RequirementReadResponse> GetTicketsByEnterpriseProject(RequirementReadRequest request);
        OperationResult<RequirementReadResponse> GetTicket(RequirementReadRequest request);
    }
}
