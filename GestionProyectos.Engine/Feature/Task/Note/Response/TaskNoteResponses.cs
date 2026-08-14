namespace GestionProyectos.Engine.Feature.Task.Note.Response;

public class TaskNoteListResponse
{
    public List<TaskNoteItem> Notes { get; set; } = new();
}

public class TaskNoteItem
{
    public long Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public long AuthorUserId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TaskNoteSaveResponse
{
    public long Id { get; set; }
}
