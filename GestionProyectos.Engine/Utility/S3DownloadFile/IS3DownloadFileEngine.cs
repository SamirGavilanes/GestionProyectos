using GestionProyectos.Engine.Utility.S3DownloadFile.Request;
using GestionProyectos.Engine.Utility.S3DownloadFile.Response;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Utility.S3DownloadFile
{
    public interface IS3DownloadFileEngine
    {
        OperationResult<S3DownloadFileResponse> Execute(S3DownloadFileRequest request);
    }
}
