namespace GestionProyectos.Engine.Feature.Task
{
    public static class TaskStatusHelper
    {
        public static bool RequiresBlockReason(string? statusDescription) =>
            !string.IsNullOrWhiteSpace(statusDescription) &&
            statusDescription.Contains("bloqueo", StringComparison.OrdinalIgnoreCase);

        public static bool IsExternalBlock(string? statusDescription) =>
            RequiresBlockReason(statusDescription) &&
            statusDescription!.Contains("externo", StringComparison.OrdinalIgnoreCase);
    }
}
