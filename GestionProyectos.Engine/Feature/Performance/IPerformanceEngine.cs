using GestionProyectos.Engine.Feature.Performance.Request;
using GestionProyectos.Engine.Feature.Performance.Response;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Feature.Performance;

public interface IPerformanceEngine
{
    OperationResult<PerformanceReadResponse> Execute(PerformanceReadRequest request);
}
