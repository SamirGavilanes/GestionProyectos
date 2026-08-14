using GestionProyectos.Engine.Feature.Task.TimeLogRegistration.Request;
using GestionProyectos.Engine.Feature.Task.TimeLogRegistration.Response;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Feature.Task.TimeLogRegistration
{
    public interface ITimeLogRegistrationEngine
    {
        OperationResult<TimeLogRegistrationResponse> Execute(TimeLogRegistrationRequest request);
    }
}
