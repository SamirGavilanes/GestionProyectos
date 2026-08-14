using GestionProyectos.Engine.Security.Request;
using GestionProyectos.Engine.Security.Response;
using GestionProyectos.Engine.Security.Utilities;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Security
{
    public interface ISecurityEngine
    {
        OperationResult<LoginResponse> Login(LoginRequest request);
        PagePermissions GetPermissionsForUser(long userId, string route);
        List<MenuItem> GetMenusForUser(long userId);
    }
}
