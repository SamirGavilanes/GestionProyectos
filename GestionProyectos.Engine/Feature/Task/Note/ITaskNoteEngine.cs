using GestionProyectos.Engine.Feature.Task.Note.Request;
using GestionProyectos.Engine.Feature.Task.Note.Response;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Feature.Task.Note;

public interface ITaskNoteEngine
{
    OperationResult<TaskNoteListResponse> GetNotes(TaskNoteListRequest request);
    OperationResult<TaskNoteSaveResponse> SaveNote(TaskNoteSaveRequest request);
    OperationResult<bool> DeleteNote(TaskNoteDeleteRequest request);
}
