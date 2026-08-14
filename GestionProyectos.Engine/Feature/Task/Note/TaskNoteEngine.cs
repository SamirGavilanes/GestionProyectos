using GestionProyectos.Data;
using GestionProyectos.Data.Entities.TaskManagement;
using GestionProyectos.Engine.Feature.Task.Note.Request;
using GestionProyectos.Engine.Feature.Task.Note.Response;
using GestionProyectos.Engine.Security.Utilities;
using GestionProyectos.Shared.Enums;
using GestionProyectos.Shared.Message;
using Microsoft.EntityFrameworkCore;

namespace GestionProyectos.Engine.Feature.Task.Note;

public class TaskNoteEngine : ITaskNoteEngine
{
    private readonly DataDbContext dbContext;

    public TaskNoteEngine(DataDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public OperationResult<TaskNoteListResponse> GetNotes(TaskNoteListRequest request)
    {
        try
        {
            var accessError = ValidateTaskAccess(request.TaskId, request.Context);
            if (accessError != null)
                return OperationResult<TaskNoteListResponse>.CreateFailureResult(accessError);

            var notes = dbContext.TaskNote
                .Include(n => n.AuthorUser)
                .Where(n => n.TaskId == request.TaskId && n.RowStatus == (short)RowStatus.Active)
                .OrderByDescending(n => n.Created)
                .ToList();

            var response = new TaskNoteListResponse();
            foreach (var note in notes)
            {
                response.Notes.Add(new TaskNoteItem
                {
                    Id = note.Id,
                    Content = note.Content,
                    Author = $"{note.AuthorUser.Name} {note.AuthorUser.LastName}".Trim(),
                    AuthorUserId = note.AuthorUserId,
                    CreatedAt = note.Created
                });
            }

            return OperationResult<TaskNoteListResponse>.CreateSuccessResult(response);
        }
        catch (Exception ex)
        {
            return OperationResult<TaskNoteListResponse>.CreateFailureResult(ex);
        }
    }

    public OperationResult<TaskNoteSaveResponse> SaveNote(TaskNoteSaveRequest request)
    {
        try
        {
            var accessError = ValidateTaskAccess(request.TaskId, request.Context);
            if (accessError != null)
                return OperationResult<TaskNoteSaveResponse>.CreateFailureResult(accessError);

            if (string.IsNullOrWhiteSpace(request.Content))
                return OperationResult<TaskNoteSaveResponse>.CreateFailureResult("La nota no puede estar vacía.");

            var note = new TaskNote
            {
                TaskId = request.TaskId,
                Content = request.Content.Trim(),
                AuthorUserId = request.Context.UserId,
                RowStatus = (short)RowStatus.Active,
                Created = DateTime.UtcNow,
                CreatedBy = request.Context.UserId
            };

            dbContext.TaskNote.Add(note);
            dbContext.SaveChanges();

            return OperationResult<TaskNoteSaveResponse>.CreateSuccessResult(new TaskNoteSaveResponse { Id = note.Id });
        }
        catch (Exception ex)
        {
            return OperationResult<TaskNoteSaveResponse>.CreateFailureResult(ex);
        }
    }

    public OperationResult<bool> DeleteNote(TaskNoteDeleteRequest request)
    {
        try
        {
            var note = dbContext.TaskNote.FirstOrDefault(n =>
                n.Id == request.Id && n.RowStatus == (short)RowStatus.Active);

            if (note == null)
                return OperationResult<bool>.CreateFailureResult("La nota no existe.");

            var accessError = ValidateTaskAccess(note.TaskId, request.Context);
            if (accessError != null)
                return OperationResult<bool>.CreateFailureResult(accessError);

            var isAuthor = note.AuthorUserId == request.Context.UserId;
            var isAdmin = request.Context.Role?.Equals("Administrador", StringComparison.OrdinalIgnoreCase) == true;
            if (!isAuthor && !isAdmin)
                return OperationResult<bool>.CreateFailureResult("Solo el autor o un administrador puede eliminar la nota.");

            note.RowStatus = (short)RowStatus.Inactive;
            note.Updated = DateTime.UtcNow;
            note.UpdatedBy = request.Context.UserId;
            dbContext.SaveChanges();

            return OperationResult<bool>.CreateSuccessResult(true);
        }
        catch (Exception ex)
        {
            return OperationResult<bool>.CreateFailureResult(ex);
        }
    }

    private string? ValidateTaskAccess(long taskId, Context context)
    {
        var task = dbContext.Task.FirstOrDefault(t =>
            t.Id == taskId && t.RowStatus == (short)RowStatus.Active);

        if (task == null)
            return "La tarea no existe.";

        if (IsDeveloperRole(context) && task.UserId != context.UserId)
            return "No tiene acceso a esta tarea.";

        return null;
    }

    private static bool IsDeveloperRole(Context context) =>
        context.Role?.Equals("Desarrollador", StringComparison.OrdinalIgnoreCase) == true;
}
