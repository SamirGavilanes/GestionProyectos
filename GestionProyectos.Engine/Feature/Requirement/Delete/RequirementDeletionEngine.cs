using GestionProyectos.Data;
using GestionProyectos.Engine.Feature.Requirement.Delete.Request;
using GestionProyectos.Engine.Feature.Requirement.Delete.Response;
using GestionProyectos.Engine.Security.Utilities;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Feature.Requirement.Delete
{
    public class RequirementDeletionEngine : IRequirementDeletionEngine
    {
        private readonly DataDbContext dbContext;

        public RequirementDeletionEngine(DataDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public OperationResult<RequirementDeletionResponse> Execute(RequirementDeletionRequest request)
        {
            try
            {
                RequirementDeletionResponse response = new();
                var ticket = dbContext.Requirement.FirstOrDefault(x => x.Id == request.TicketId);

                if (ticket == null)
                    return OperationResult<RequirementDeletionResponse>.CreateFailureResult("El Ticket que quiere borrar ya no existe.");

                dbContext.Requirement.Remove(ticket);
                dbContext.SaveChanges();

                return OperationResult<RequirementDeletionResponse>.CreateSuccessResult(response);

            }
            catch (Exception ex)
            {
                return OperationResult<RequirementDeletionResponse>.CreateFailureResult(ex);

            }
        }
    }
}
