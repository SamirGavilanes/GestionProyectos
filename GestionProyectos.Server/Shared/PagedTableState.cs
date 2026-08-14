using Microsoft.AspNetCore.Components;

namespace GestionProyectos.Server.Shared;

public static class TableSearch
{
    public static bool Matches(string search, params string?[] values)
    {
        if (string.IsNullOrWhiteSpace(search)) return true;
        var s = search.ToLower();
        return values.Any(v => (v ?? string.Empty).ToLower().Contains(s));
    }
}

public class PagedTableState
{
    public const int PageSize = 15;

    public int CurrentPage { get; set; } = 1;
    public int TotalItems { get; set; }
    public int TotalPages { get; set; } = 1;
    public string? SearchText { get; set; }
    public string SortColumn { get; set; } = "Id";
    public bool SortAscending { get; set; } = true;

    public void SetPageCount(int totalItems)
    {
        TotalItems = totalItems;
        TotalPages = Math.Max(1, (int)Math.Ceiling(TotalItems / (double)PageSize));
        if (CurrentPage > TotalPages) CurrentPage = TotalPages;
        if (CurrentPage < 1) CurrentPage = 1;
    }

    public IEnumerable<T> Paginate<T>(IReadOnlyList<T> items) =>
        items.Skip((CurrentPage - 1) * PageSize).Take(PageSize);

    public void SortBy(string column)
    {
        if (SortColumn == column)
            SortAscending = !SortAscending;
        else
        {
            SortColumn = column;
            SortAscending = true;
        }
        CurrentPage = 1;
    }

    public void GoToPage(int page)
    {
        if (page >= 1 && page <= TotalPages)
            CurrentPage = page;
    }

    public void ResetPage() => CurrentPage = 1;

    public MarkupString SortIcon(string column)
    {
        if (SortColumn != column)
            return new MarkupString("<span class=\"text-gray-300\">↕</span>");
        return new MarkupString(SortAscending
            ? "<span class=\"text-primary-600\">↑</span>"
            : "<span class=\"text-primary-600\">↓</span>");
    }
}
