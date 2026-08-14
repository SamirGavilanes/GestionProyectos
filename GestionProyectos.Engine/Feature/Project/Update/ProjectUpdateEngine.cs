using GestionProyectos.Data;
using GestionProyectos.Engine.Feature.Project.Update.Request;
using GestionProyectos.Engine.Feature.Project.Update.Response;
using GestionProyectos.Engine.Feature;
using GestionProyectos.Engine.Security.Utilities;
using GestionProyectos.Shared.Enums;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Feature.Project.Update
{
    public class ProjectUpdateEngine : IProjectUpdateEngine
    {
        private readonly DataDbContext dbContext;
        public ProjectUpdateEngine(DataDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public OperationResult<ProjectUpdateResponse> Execute(ProjectUpdateRequest request)
        {
            try
            {
                var project = dbContext.Project.FirstOrDefault(x => x.Id == request.Id);

                if (project == null)
                    return OperationResult<ProjectUpdateResponse>.CreateFailureResult("El item seleccionado no existe.");

                if (!request.EndDate.HasValue)
                    return OperationResult<ProjectUpdateResponse>.CreateFailureResult("La fecha límite es obligatoria.");

                if (request.EndDate.Value.Date < request.StartDate.Date)
                    return OperationResult<ProjectUpdateResponse>.CreateFailureResult("La fecha límite no puede ser anterior al inicio.");

                if (request.ProjectStatusId != project.ProjectStatusId)
                {
                    var newStatus = dbContext.ProjectStatus
                        .FirstOrDefault(s => s.Id == request.ProjectStatusId && s.RowStatus == (short)RowStatus.Active);
                    if (newStatus != null &&
                        FinalizeStatusHelper.IsProjectFinalized(newStatus.IsClosed) &&
                        !PermissionHelper.CanFinalizeProject(request.Context))
                        return OperationResult<ProjectUpdateResponse>.CreateFailureResult("No tiene permiso para finalizar proyectos.");
                }

                project.ProjectStatusId = request.ProjectStatusId;
                project.CustomerId = request.CustomerId;
                project.Description = request.Description;
                project.StartDate = request.StartDate;
                project.EndDate = request.EndDate;
                project.ActualEndDate = request.ActualEndDate;
                project.Updated = DateTime.UtcNow;
                project.UpdatedBy = request.Context.UserId;

                dbContext.SaveChanges();

                return OperationResult<ProjectUpdateResponse>.CreateSuccessResult(new ProjectUpdateResponse());
            }
            catch (Exception ex)
            {
                return OperationResult<ProjectUpdateResponse>.CreateFailureResult(ex);
            }
        }
    }
}
