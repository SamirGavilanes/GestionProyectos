using GestionProyectos.Server.Models;

namespace GestionProyectos.Server.Extensions;

public static class FacetCountExtensions
{
    public static List<FacetOption> ToFacetOptions<T>(
        this IEnumerable<(long Id, string Label)> catalog,
        IEnumerable<T> source,
        Func<T, long> keySelector,
        Func<T, bool> countPredicate,
        long selectedId) =>
        catalog.ToFacetOptions(source, keySelector, countPredicate, ToSelectedSet(selectedId));

    public static List<FacetOption> ToFacetOptions<T>(
        this IEnumerable<(long Id, string Label)> catalog,
        IEnumerable<T> source,
        Func<T, long> keySelector,
        Func<T, bool> countPredicate,
        IReadOnlyCollection<long> selectedIds)
    {
        var selectedSet = selectedIds as HashSet<long> ?? new HashSet<long>(selectedIds);
        var counts = source
            .Where(countPredicate)
            .GroupBy(keySelector)
            .ToDictionary(g => g.Key, g => g.Count());

        return catalog
            .Select(item =>
            {
                counts.TryGetValue(item.Id, out var count);
                return new FacetOption { Id = item.Id, Label = item.Label, Count = count };
            })
            .Where(o => o.Count > 0 || selectedSet.Contains(o.Id))
            .OrderByDescending(o => selectedSet.Contains(o.Id))
            .ThenBy(o => o.Label, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public static List<FacetOption> ToFacetOptionsByName<T>(
        this IEnumerable<(long Id, string Label)> catalog,
        IEnumerable<T> source,
        Func<T, string> nameSelector,
        Func<T, bool> countPredicate,
        long selectedId) =>
        catalog.ToFacetOptionsByName(source, nameSelector, countPredicate, ToSelectedSet(selectedId));

    public static List<FacetOption> ToFacetOptionsByName<T>(
        this IEnumerable<(long Id, string Label)> catalog,
        IEnumerable<T> source,
        Func<T, string> nameSelector,
        Func<T, bool> countPredicate,
        IReadOnlyCollection<long> selectedIds)
    {
        var selectedSet = selectedIds as HashSet<long> ?? new HashSet<long>(selectedIds);
        var counts = source
            .Where(countPredicate)
            .GroupBy(nameSelector, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.Count(), StringComparer.OrdinalIgnoreCase);

        return catalog
            .Select(item =>
            {
                counts.TryGetValue(item.Label, out var count);
                return new FacetOption { Id = item.Id, Label = item.Label, Count = count };
            })
            .Where(o => o.Count > 0 || selectedSet.Contains(o.Id))
            .OrderByDescending(o => selectedSet.Contains(o.Id))
            .ThenBy(o => o.Label, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static HashSet<long> ToSelectedSet(long selectedId) =>
        selectedId > 0 ? new HashSet<long> { selectedId } : new HashSet<long>();
}
