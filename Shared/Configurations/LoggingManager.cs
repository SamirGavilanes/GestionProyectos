namespace GestionProyectos.Shared.Configurations
{
    public class LoggingManager
    {
        public string LoggerHubURL { get; set; } = string.Empty;
        public string LoggerFilePath { get; set; } = string.Empty;
        public long LoggerApplicationId { get; set; }
        public string Reference { get; set; } = string.Empty;
    }
}
