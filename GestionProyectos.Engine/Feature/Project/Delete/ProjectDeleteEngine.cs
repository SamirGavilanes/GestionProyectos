using GestionProyectos.Data;
using GestionProyectos.Engine.Feature.Project.Delete.Request;
using GestionProyectos.Engine.Feature.Project.Delete.Response;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Feature.Project.Delete
{
    public class ProjectDeleteEngine : IProjectDeleteEngine
    {
        private readonly DataDbContext dbContext;
        public ProjectDeleteEngine(DataDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public OperationResult<ProjectDeleteResponse> Execute(ProjectDeleteRequest request)
        {
            try
            {
                // OBTENER EL PROYECTO CON EL ID
                var project = dbContext.Project.FirstOrDefault(x => x.Id == request.ProjectId);

                // VALIDAR QUE EL PROYECTO EXISTA
                if (project == null)
                    return OperationResult<ProjectDeleteResponse>.CreateFailureResult("El registro ya fue eliminado.");

                // VALIDAR QUE NO TENGA REQUERIMIENTOS
                if (project.Requirements != null && project.Requirements.Count > 0)
                    return OperationResult<ProjectDeleteResponse>.CreateFailureResult("El proyecto tiene requerimientos ingresados.");

                // BORRA REQISTRO
                dbContext.Project.Remove(project);
                dbContext.SaveChanges();

                return OperationResult<ProjectDeleteResponse>.CreateSuccessResult(new ProjectDeleteResponse());
            }
            catch (Exception ex)
            {
                return OperationResult<ProjectDeleteResponse>.CreateFailureResult(ex);
            }
        }
    }
}
