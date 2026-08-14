using GestionProyectos.Engine.Utility.S3UploadFile.Request;
using GestionProyectos.Engine.Utility.S3UploadFile.Response;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Utility.S3UploadFile
{
    public interface IS3UploadFileEngine
    {
        OperationResult<S3UploadFileResponse> Execute(S3UploadFileRequest request);
    }
}
