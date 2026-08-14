namespace GestionProyectos.Shared.Enums
{
    public class Constants
    {
        public static readonly string EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? string.Empty;
    }
}
