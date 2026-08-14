using GestionProyectos.Engine.Excel.Download.Request;
using GestionProyectos.Engine.Excel.Download.Response;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Excel.Download
{
    public interface IExcelDownloadEngine
    {
        OperationResult<ExcelDownloadResponse> Download(ExcelDownloadRequest request);
    }
}
