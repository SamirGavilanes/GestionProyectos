using GestionProyectos.Data;
using GestionProyectos.Engine.Feature;
using GestionProyectos.Engine.Feature.Project.Create.Request;
using GestionProyectos.Engine.Feature.Project.Create.Response;
using GestionProyectos.Engine.Security.Utilities;
using GestionProyectos.Shared.Enums;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Feature.Project.Create
{
    public class ProjectCreateEngine : IProjectCreateEngine
    {
        private readonly DataDbContext dbContext;
        public ProjectCreateEngine(DataDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public OperationResult<ProjectCreateResponse> Execute(ProjectCreateRequest request)
        {
            try
            {
                var status = dbContext.ProjectStatus
                    .FirstOrDefault(s => s.Id == request.ProjectStatusId && s.RowStatus == (short)RowStatus.Active);
                if (status != null &&
                    FinalizeStatusHelper.IsProjectFinalized(status.IsClosed) &&
                    !PermissionHelper.CanFinalizeProject(request.Context))
                    return OperationResult<ProjectCreateResponse>.CreateFailureResult("No tiene permiso para finalizar proyectos.");

                if (!request.EndDate.HasValue)
                    return OperationResult<ProjectCreateResponse>.CreateFailureResult("La fecha límite es obligatoria.");

                if (request.EndDate.Value.Date < request.StartDate.Date)
                    return OperationResult<ProjectCreateResponse>.CreateFailureResult("La fecha límite no puede ser anterior al inicio.");

                Data.Entities.TaskManagement.Project project = new()
                {
                    Description = request.Description,
                    CustomerId = request.CustomerId,
                    RowStatus = (short)RowStatus.Active,
                    Created = DateTime.UtcNow,
                    CreatedBy = request.Context.UserId,
                    ProjectStatusId = request.ProjectStatusId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    ActualEndDate = request.ActualEndDate,
                };

                dbContext.Project.Add(project);
                dbContext.SaveChanges();

                ProjectCreateResponse response = new();
                return OperationResult<ProjectCreateResponse>.CreateSuccessResult(response);
            }
            catch (Exception ex)
            {
                return OperationResult<ProjectCreateResponse>.CreateFailureResult(ex);
            }
        }
    }
}
