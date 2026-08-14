using GestionProyectos.Engine.Excel.Download.Response;
using GestionProyectos.Engine.Feature.Billing.Request;
using GestionProyectos.Engine.Feature.Billing.Response;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Feature.Billing;

public interface IBillingReportEngine
{
    OperationResult<BillingReportResponse> Execute(BillingReportRequest request);
    OperationResult<ExcelDownloadResponse> Export(BillingExportRequest request);
}
