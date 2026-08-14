using GestionProyectos.Engine.Feature.Dashboard.Request;
using GestionProyectos.Engine.Feature.Dashboard.Response;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Feature.Dashboard;

public interface IDashboardReadEngine
{
    OperationResult<DashboardReadResponse> Execute(DashboardReadRequest request);
}
