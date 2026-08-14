using GestionProyectos.Data;
using GestionProyectos.Data.Entities.TaskManagement;
using GestionProyectos.Engine.Feature.Task.Bug.Request;
using GestionProyectos.Engine.Feature.Task.Bug.Response;
using GestionProyectos.Engine.Security.Utilities;
using GestionProyectos.Engine.Utility.S3DownloadFile;
using GestionProyectos.Engine.Utility.S3DownloadFile.Request;
using GestionProyectos.Engine.Utility.S3UploadFile;
using GestionProyectos.Engine.Utility.S3UploadFile.Request;
using GestionProyectos.Shared.Configurations;
using GestionProyectos.Shared.Enums;
using GestionProyectos.Shared.Message;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GestionProyectos.Engine.Feature.Task.Bug;

public class TaskBugEngine : ITaskBugEngine
{
    private const string BugListRoute = "/task-bug-list";

    private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".png", ".jpg", ".jpeg", ".gif", ".webp", ".bmp"
    };

    private readonly DataDbContext dbContext;
    private readonly IS3UploadFileEngine s3UploadFileEngine;
    private readonly IS3DownloadFileEngine s3DownloadFileEngine;
    private readonly IOptions<AppSettingsManagerBase> appSettings;

    public TaskBugEngine(
        DataDbContext dbContext,
        IS3UploadFileEngine s3UploadFileEngine,
        IS3DownloadFileEngine s3DownloadFileEngine,
        IOptions<AppSettingsManagerBase> appSettings)
    {
        this.dbContext = dbContext;
        this.s3UploadFileEngine = s3UploadFileEngine;
        this.s3DownloadFileEngine = s3DownloadFileEngine;
        this.appSettings = appSettings;
    }

    public OperationResult<TaskBugListResponse> GetBugs(TaskBugListRequest request)
    {
        try
        {
            var accessError = ValidateTaskAccess(request.TaskId, request.Context);
            if (accessError != null)
                return OperationResult<TaskBugListResponse>.CreateFailureResult(accessError);

            var statuses = GetStatusMap();
            var bugs = dbContext.TaskBug
                .Include(b => b.ReportedByUser)
                .Where(b => b.TaskId == request.TaskId && b.RowStatus == (short)RowStatus.Active)
                .OrderByDescending(b => b.Created)
                .ToList();

            var response = new TaskBugListResponse();
            foreach (var bug in bugs)
            {
                response.Bugs.Add(MapBugItem(bug, statuses));
            }

            return OperationResult<TaskBugListResponse>.CreateSuccessResult(response);
        }
        catch (Exception ex)
        {
            return OperationResult<TaskBugListResponse>.CreateFailureResult(ex);
        }
    }

    public OperationResult<TaskBugGlobalListResponse> GetAllBugs(TaskBugGlobalListRequest request)
    {
        try
        {
            var statuses = GetStatusMap();

            var query = dbContext.TaskBug
                .Include(b => b.ReportedByUser)
                .Include(b => b.Requirement)
                    .ThenInclude(r => r.Project)
                        .ThenInclude(p => p.Customer)
                .Include(b => b.Task)
                    .ThenInclude(t => t!.User)
                .Where(b => b.RowStatus == (short)RowStatus.Active);

            if (IsDeveloperRole(request.Context))
            {
                var developerId = request.Context.UserId;
                query = query.Where(b =>
                    b.ReportedByUserId == developerId
                    || (b.TaskId != null && b.Task!.UserId == developerId));
            }
            else if (request.DeveloperId > 0)
                query = query.Where(b => b.TaskId != null && b.Task!.UserId == request.DeveloperId);

            if (request.EnterpriseId > 0)
                query = query.Where(b => b.Requirement.Project.Customer.EnterpriseId == request.EnterpriseId);

            if (request.CustomerId > 0)
                query = query.Where(b => b.Requirement.Project.CustomerId == request.CustomerId);

            if (request.ProjectId > 0)
                query = query.Where(b => b.Requirement.ProjectId == request.ProjectId);

            if (request.TaskBugStatusId > 0)
                query = query.Where(b => b.TaskBugStatusId == request.TaskBugStatusId);

            var bugs = query
                .OrderByDescending(b => b.Created)
                .ToList();

            var response = new TaskBugGlobalListResponse();
            foreach (var bug in bugs)
            {
                var attachments = dbContext.TaskBugAttachment
                    .Where(a => a.TaskBugId == bug.Id && a.RowStatus == (short)RowStatus.Active)
                    .OrderBy(a => a.Id)
                    .Select(a => new TaskBugAttachmentItem { Id = a.Id, FileName = a.FileName })
                    .ToList();

                response.Bugs.Add(new TaskBugOverviewItem
                {
                    Id = bug.Id,
                    RequirementId = bug.RequirementId,
                    RequirementDescription = bug.Requirement?.Description ?? string.Empty,
                    IsWithinOriginalScope = bug.Requirement?.IsWithinOriginalScope ?? true,
                    TaskId = bug.TaskId,
                    TaskDescription = bug.Task?.Description ?? string.Empty,
                    DeveloperId = bug.Task?.UserId ?? 0,
                    DeveloperName = bug.Task?.User != null
                        ? $"{bug.Task.User.Name} {bug.Task.User.LastName}".Trim()
                        : string.Empty,
                    Description = bug.Description,
                    TaskBugStatusId = bug.TaskBugStatusId,
                    Status = statuses.TryGetValue(bug.TaskBugStatusId, out var status) ? status : bug.TaskBugStatusId.ToString(),
                    ReportedBy = $"{bug.ReportedByUser.Name} {bug.ReportedByUser.LastName}".Trim(),
                    ReportedAt = bug.StartDate == default ? bug.Created : bug.StartDate,
                    StartDate = bug.StartDate == default ? bug.Created : bug.StartDate,
                    EndDate = bug.EndDate == default ? bug.Created : bug.EndDate,
                    Attachments = attachments
                });
            }

            return OperationResult<TaskBugGlobalListResponse>.CreateSuccessResult(response);
        }
        catch (Exception ex)
        {
            return OperationResult<TaskBugGlobalListResponse>.CreateFailureResult(ex);
        }
    }

    public OperationResult<TaskBugTaskOptionsResponse> GetTasksForRequirement(long requirementId, Context context)
    {
        try
        {
            if (!PermissionHelper.CanEdit(context, BugListRoute))
                return OperationResult<TaskBugTaskOptionsResponse>.CreateFailureResult("No tiene permiso para asignar bugs a tareas.");

            var requirementExists = dbContext.Requirement.Any(r =>
                r.Id == requirementId && r.RowStatus == (short)RowStatus.Active);
            if (!requirementExists)
                return OperationResult<TaskBugTaskOptionsResponse>.CreateFailureResult("El requerimiento no existe.");

            var tasksQuery = dbContext.Task
                .Include(t => t.User)
                .Where(t => t.RequirementId == requirementId && t.RowStatus == (short)RowStatus.Active);

            if (IsDeveloperRole(context))
                tasksQuery = tasksQuery.Where(t => t.UserId == context.UserId);

            var tasks = tasksQuery
                .OrderBy(t => t.Id)
                .Select(t => new TaskBugTaskOption
                {
                    Id = t.Id,
                    Description = t.Description,
                    DeveloperName = t.User == null ? string.Empty : (t.User.Name + " " + t.User.LastName).Trim()
                })
                .ToList();

            return OperationResult<TaskBugTaskOptionsResponse>.CreateSuccessResult(
                new TaskBugTaskOptionsResponse { Tasks = tasks });
        }
        catch (Exception ex)
        {
            return OperationResult<TaskBugTaskOptionsResponse>.CreateFailureResult(ex);
        }
    }

    public OperationResult<TaskBugSaveResponse> SaveBug(TaskBugSaveRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Description))
                return OperationResult<TaskBugSaveResponse>.CreateFailureResult("La descripción del bug es obligatoria.");

            long statusId = request.TaskBugStatusId;
            if (request.Id <= 0)
            {
                statusId = dbContext.TaskBugStatus
                    .Where(s => s.RowStatus == (short)RowStatus.Active && s.Description == "Reportado")
                    .Select(s => (long?)s.Id)
                    .FirstOrDefault()
                    ?? dbContext.TaskBugStatus
                        .Where(s => s.RowStatus == (short)RowStatus.Active)
                        .OrderBy(s => s.Order)
                        .ThenBy(s => s.Id)
                        .Select(s => (long?)s.Id)
                        .FirstOrDefault()
                    ?? 0;
            }

            if (statusId <= 0)
                return OperationResult<TaskBugSaveResponse>.CreateFailureResult(
                    request.Id <= 0
                        ? "No está configurado el estado Reportado para bugs."
                        : "Seleccione el estado del bug.");

            var statusExists = dbContext.TaskBugStatus.Any(s =>
                s.Id == statusId && s.RowStatus == (short)RowStatus.Active);
            if (!statusExists)
                return OperationResult<TaskBugSaveResponse>.CreateFailureResult("El estado del bug no es válido.");

            foreach (var file in request.Files)
            {
                var ext = Path.GetExtension(file.Name);
                if (!AllowedImageExtensions.Contains(ext))
                    return OperationResult<TaskBugSaveResponse>.CreateFailureResult(
                        $"Solo se permiten imágenes ({string.Join(", ", AllowedImageExtensions)}). Archivo: {file.Name}");
            }

            TaskBug bug;
            long requirementId;
            long? taskId;

            if (request.Id > 0)
            {
                if (!PermissionHelper.CanEdit(request.Context, BugListRoute))
                    return OperationResult<TaskBugSaveResponse>.CreateFailureResult("No tiene permiso para editar bugs.");

                var existingBug = dbContext.TaskBug.FirstOrDefault(b =>
                    b.Id == request.Id && b.RowStatus == (short)RowStatus.Active);

                if (existingBug == null)
                    return OperationResult<TaskBugSaveResponse>.CreateFailureResult("El bug no existe.");

                bug = existingBug;

                var accessError = ValidateBugAccess(bug, request.Context);
                if (accessError != null)
                    return OperationResult<TaskBugSaveResponse>.CreateFailureResult(accessError);

                bug.Description = request.Description.Trim();
                if (bug.TaskBugStatusId != statusId)
                {
                    bug.TaskBugStatusId = statusId;
                    bug.EndDate = DateTime.UtcNow;
                }
                bug.Updated = DateTime.UtcNow;
                bug.UpdatedBy = request.Context.UserId;

                requirementId = bug.RequirementId;
                taskId = bug.TaskId;
            }
            else
            {
                if (!PermissionHelper.CanCreate(request.Context, BugListRoute))
                    return OperationResult<TaskBugSaveResponse>.CreateFailureResult("No tiene permiso para reportar bugs.");

                if (request.TaskId.HasValue && request.TaskId.Value > 0)
                {
                    var task = dbContext.Task.FirstOrDefault(t =>
                        t.Id == request.TaskId.Value && t.RowStatus == (short)RowStatus.Active);
                    if (task == null)
                        return OperationResult<TaskBugSaveResponse>.CreateFailureResult("La tarea no existe.");

                    var taskAccessError = ValidateTaskAccess(task.Id, request.Context);
                    if (taskAccessError != null)
                        return OperationResult<TaskBugSaveResponse>.CreateFailureResult(taskAccessError);

                    requirementId = task.RequirementId;
                    taskId = task.Id;
                }
                else
                {
                    if (request.RequirementId <= 0)
                        return OperationResult<TaskBugSaveResponse>.CreateFailureResult("Seleccione un requerimiento.");

                    var requirementExists = dbContext.Requirement.Any(r =>
                        r.Id == request.RequirementId && r.RowStatus == (short)RowStatus.Active);
                    if (!requirementExists)
                        return OperationResult<TaskBugSaveResponse>.CreateFailureResult("El requerimiento no existe.");

                    requirementId = request.RequirementId;
                    taskId = null;
                }

                var now = DateTime.UtcNow;
                bug = new TaskBug
                {
                    RequirementId = requirementId,
                    TaskId = taskId,
                    Description = request.Description.Trim(),
                    TaskBugStatusId = statusId,
                    ReportedByUserId = request.Context.UserId,
                    StartDate = now,
                    EndDate = now,
                    RowStatus = (short)RowStatus.Active,
                    Created = now,
                    CreatedBy = request.Context.UserId
                };
                dbContext.TaskBug.Add(bug);
            }

            dbContext.SaveChanges();

            if (request.Files.Count > 0)
            {
                var destinationPath = BuildAttachmentPath(requirementId, taskId, bug.Id);
                UploadAttachments(request, bug, destinationPath);
            }

            return OperationResult<TaskBugSaveResponse>.CreateSuccessResult(new TaskBugSaveResponse { Id = bug.Id });
        }
        catch (Exception ex)
        {
            return OperationResult<TaskBugSaveResponse>.CreateFailureResult(ex);
        }
    }

    public OperationResult<bool> AssignBugToTask(TaskBugAssignRequest request)
    {
        try
        {
            if (!IsAdministrator(request.Context))
                return OperationResult<bool>.CreateFailureResult("Solo un administrador puede asignar bugs a tareas.");

            if (request.TaskId <= 0)
                return OperationResult<bool>.CreateFailureResult("Seleccione una tarea.");

            var bug = dbContext.TaskBug.FirstOrDefault(b =>
                b.Id == request.BugId && b.RowStatus == (short)RowStatus.Active);
            if (bug == null)
                return OperationResult<bool>.CreateFailureResult("El bug no existe.");

            if (bug.TaskId.HasValue && bug.TaskId.Value > 0)
                return OperationResult<bool>.CreateFailureResult("El bug ya está asignado a una tarea.");

            var task = dbContext.Task.FirstOrDefault(t =>
                t.Id == request.TaskId && t.RowStatus == (short)RowStatus.Active);
            if (task == null)
                return OperationResult<bool>.CreateFailureResult("La tarea no existe.");

            if (task.RequirementId != bug.RequirementId)
                return OperationResult<bool>.CreateFailureResult("La tarea debe pertenecer al mismo requerimiento del bug.");

            bug.TaskId = task.Id;
            bug.Updated = DateTime.UtcNow;
            bug.UpdatedBy = request.Context.UserId;
            dbContext.SaveChanges();

            return OperationResult<bool>.CreateSuccessResult(true);
        }
        catch (Exception ex)
        {
            return OperationResult<bool>.CreateFailureResult(ex);
        }
    }

    public OperationResult<bool> DeleteBug(TaskBugDeleteRequest request)
    {
        try
        {
            var bug = dbContext.TaskBug.FirstOrDefault(b =>
                b.Id == request.Id && b.RowStatus == (short)RowStatus.Active);

            if (bug == null)
                return OperationResult<bool>.CreateFailureResult("El bug no existe.");

            var accessError = ValidateBugAccess(bug, request.Context);
            if (accessError != null)
                return OperationResult<bool>.CreateFailureResult(accessError);

            if (!PermissionHelper.CanDelete(request.Context, BugListRoute))
                return OperationResult<bool>.CreateFailureResult("No tiene permiso para eliminar bugs.");

            bug.RowStatus = (short)RowStatus.Inactive;
            bug.Updated = DateTime.UtcNow;
            bug.UpdatedBy = request.Context.UserId;

            var attachments = dbContext.TaskBugAttachment
                .Where(a => a.TaskBugId == bug.Id && a.RowStatus == (short)RowStatus.Active)
                .ToList();

            foreach (var attachment in attachments)
            {
                attachment.RowStatus = (short)RowStatus.Inactive;
                attachment.Updated = DateTime.UtcNow;
                attachment.UpdatedBy = request.Context.UserId;
            }

            dbContext.SaveChanges();
            return OperationResult<bool>.CreateSuccessResult(true);
        }
        catch (Exception ex)
        {
            return OperationResult<bool>.CreateFailureResult(ex);
        }
    }

    public OperationResult<TaskBugDownloadResponse> DownloadAttachment(TaskBugDownloadRequest request)
    {
        try
        {
            var attachment = dbContext.TaskBugAttachment.FirstOrDefault(a =>
                a.Id == request.AttachmentId && a.RowStatus == (short)RowStatus.Active);

            if (attachment == null)
                return OperationResult<TaskBugDownloadResponse>.CreateFailureResult("El adjunto no existe.");

            var bug = dbContext.TaskBug.FirstOrDefault(b => b.Id == attachment.TaskBugId);
            if (bug == null)
                return OperationResult<TaskBugDownloadResponse>.CreateFailureResult("El bug no existe.");

            var accessError = ValidateBugAccess(bug, request.Context);
            if (accessError != null)
                return OperationResult<TaskBugDownloadResponse>.CreateFailureResult(accessError);

            var downloadRequest = new S3DownloadFileRequest
            {
                AccessKey = appSettings.Value.Configurations.S3Config.AccessKey,
                BuketName = appSettings.Value.Configurations.S3Config.BuketName,
                SecretAccessKey = appSettings.Value.Configurations.S3Config.SecretAccessKey,
                Active = appSettings.Value.Configurations.S3Config.Active,
                FilePath = $"{attachment.FilePath}{attachment.FileName}"
            };

            var fileResponse = s3DownloadFileEngine.Execute(downloadRequest);
            if (!fileResponse.Success)
                return OperationResult<TaskBugDownloadResponse>.CreateFailureResult(fileResponse.Message.Description);

            if (fileResponse.Data == null)
                return OperationResult<TaskBugDownloadResponse>.CreateFailureResult("No se pudo descargar el archivo.");

            return OperationResult<TaskBugDownloadResponse>.CreateSuccessResult(new TaskBugDownloadResponse
            {
                FileName = attachment.FileName,
                File = fileResponse.Data.File
            });
        }
        catch (Exception ex)
        {
            return OperationResult<TaskBugDownloadResponse>.CreateFailureResult(ex);
        }
    }

    private void UploadAttachments(TaskBugSaveRequest request, TaskBug bug, string destinationPath)
    {
        foreach (var file in request.Files)
        {
            var uploadRequest = new S3UploadFileRequest
            {
                AccessKey = appSettings.Value.Configurations.S3Config.AccessKey,
                BuketName = appSettings.Value.Configurations.S3Config.BuketName,
                SecretAccessKey = appSettings.Value.Configurations.S3Config.SecretAccessKey,
                DestinationPath = destinationPath,
                Active = appSettings.Value.Configurations.S3Config.Active,
                Name = file.Name,
                File = file.File
            };
            s3UploadFileEngine.Execute(uploadRequest);

            dbContext.TaskBugAttachment.Add(new TaskBugAttachment
            {
                TaskBugId = bug.Id,
                FileName = file.Name,
                FilePath = destinationPath,
                RowStatus = (short)RowStatus.Active,
                Created = DateTime.UtcNow,
                CreatedBy = request.Context.UserId
            });
        }

        dbContext.SaveChanges();
    }

    private static string BuildAttachmentPath(long requirementId, long? taskId, long bugId) =>
        taskId.HasValue && taskId.Value > 0
            ? $"GestionProyectosQA/tasks/{taskId.Value}/bugs/{bugId}/"
            : $"GestionProyectosQA/requirements/{requirementId}/bugs/{bugId}/";

    private Dictionary<long, string> GetStatusMap() =>
        dbContext.TaskBugStatus
            .Where(s => s.RowStatus == (short)RowStatus.Active)
            .ToDictionary(s => s.Id, s => s.Description);

    private TaskBugItem MapBugItem(TaskBug bug, Dictionary<long, string> statuses)
    {
        var attachments = dbContext.TaskBugAttachment
            .Where(a => a.TaskBugId == bug.Id && a.RowStatus == (short)RowStatus.Active)
            .OrderBy(a => a.Id)
            .Select(a => new TaskBugAttachmentItem { Id = a.Id, FileName = a.FileName })
            .ToList();

        return new TaskBugItem
        {
            Id = bug.Id,
            Description = bug.Description,
            TaskBugStatusId = bug.TaskBugStatusId,
            Status = statuses.TryGetValue(bug.TaskBugStatusId, out var status) ? status : bug.TaskBugStatusId.ToString(),
            ReportedBy = $"{bug.ReportedByUser.Name} {bug.ReportedByUser.LastName}".Trim(),
            ReportedAt = bug.StartDate == default ? bug.Created : bug.StartDate,
            StartDate = bug.StartDate == default ? bug.Created : bug.StartDate,
            EndDate = bug.EndDate == default ? bug.Created : bug.EndDate,
            Attachments = attachments
        };
    }

    private string? ValidateBugAccess(TaskBug bug, Context context)
    {
        if (!bug.TaskId.HasValue || bug.TaskId.Value <= 0)
            return null;

        return ValidateTaskAccess(bug.TaskId.Value, context);
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

    private static bool IsAdministrator(Context context) =>
        context.Role?.Equals("Administrador", StringComparison.OrdinalIgnoreCase) == true;
}
