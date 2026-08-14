namespace GestionProyectos.Server.Extensions;

public static class FacetFilter
{
    public static bool IsActive(IReadOnlyCollection<long> selectedIds, int totalOptions) =>
        totalOptions > 0 && selectedIds.Count > 0 && selectedIds.Count < totalOptions;

    public static bool Matches(long itemId, IReadOnlyCollection<long> selectedIds, int totalOptions) =>
        !IsActive(selectedIds, totalOptions) || selectedIds.Contains(itemId);

    public static long ToApiSingleId(IReadOnlyCollection<long> selectedIds) =>
        selectedIds.Count == 1 ? selectedIds.First() : 0L;
}
