using GestionProyectos.Engine.Utility.SendEmail.Request;
using GestionProyectos.Engine.Utility.SendEmail.Response;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Utility.SendEmail
{
    public interface ISendEmailEngine
    {
        OperationResult<SendEmailResponse> Execute(SendEmailRequest request);
    }
}
