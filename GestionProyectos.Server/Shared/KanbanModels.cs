namespace GestionProyectos.Server.Shared;

public class KanbanColumn
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string BadgeColor { get; set; } = "gray";
}

public class KanbanMoveEventArgs<T>
{
    public T Item { get; set; } = default!;
    public long NewStatusId { get; set; }
    public long PreviousStatusId { get; set; }
}
