namespace GestionProyectos.Engine.Feature
{
    public static class FinalizeStatusHelper
    {
        public static bool IsTaskFinalized(string? statusDescription) =>
            ContainsFinalizeKeyword(statusDescription);

        public static bool IsRequirementFinalized(bool isClosed) => isClosed;

        public static bool IsRequirementFinalized(string? statusDescription) =>
            ContainsFinalizeKeyword(statusDescription);

        public static bool IsProjectFinalized(bool isClosed) => isClosed;

        private static bool ContainsFinalizeKeyword(string? statusDescription) =>
            !string.IsNullOrWhiteSpace(statusDescription) &&
            (statusDescription.Contains("finaliz", StringComparison.OrdinalIgnoreCase) ||
             statusDescription.Contains("complet", StringComparison.OrdinalIgnoreCase) ||
             statusDescription.Contains("cerrad", StringComparison.OrdinalIgnoreCase));
    }
}
