using GestionProyectos.Engine.Feature.HoursReport.Request;
using GestionProyectos.Engine.Feature.HoursReport.Response;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Feature.HoursReport;

public interface IHoursReportEngine
{
    OperationResult<HoursReportReadResponse> Execute(HoursReportReadRequest request);
}
