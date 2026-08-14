using GestionProyectos.Engine.Security.Utilities;

namespace GestionProyectos.Engine.Security.Response
{
    public class LoginResponse
    {
        public Context Session { get; set; } = null!;
    }
}
