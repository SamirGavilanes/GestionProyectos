using GestionProyectos.Engine.Feature.Requirement.DownloadFile.Request;
using GestionProyectos.Engine.Feature.Requirement.DownloadFile.Response;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Feature.Requirement.DownloadFile
{
    public interface IDownloadFileEngine
    {
        OperationResult<DownloadFileResponse> Execute(DownloadFileRequest request);
    }
}
