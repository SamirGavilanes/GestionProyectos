using GestionProyectos.Data;
using GestionProyectos.Engine.Feature.Project.Read.Request;
using GestionProyectos.Engine.Feature.Project.Read.Response;
using GestionProyectos.Shared.Enums;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Feature.Project.Read
{
    public class ProjectReadEngine : IProjectReadEngine
    {
        private readonly DataDbContext dbContext;

        public ProjectReadEngine(DataDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public OperationResult<ProjectReadResponse> GetProjects()
        {
            try
            {
                ProjectReadResponse response = new();

                var projects = dbContext.Project.Where(p => p.RowStatus == (short)RowStatus.Active)
                                                  .OrderBy(p => p.Description)
                                                  .ToList();

                // SE MAPEA LA RESPUESTA
                List<CommonObject.Project> projectList = new();
                foreach (var project in projects)
                {

                    CommonObject.Project projectItem = new()
                    {
                        Id = project.Id,
                        Description = project.Description,
                        Customer = project.Customer.Description
                    };

                    projectList.Add(projectItem);
                }

                // RESPUESTA
                response.Projects = projectList;
                return OperationResult<ProjectReadResponse>.CreateSuccessResult(response);
            }
            catch (Exception ex)
            {
                return OperationResult<ProjectReadResponse>.CreateFailureResult(ex);
            }
        }

        public OperationResult<ProjectReadResponse> GetProjectsByCustomer(ProjectReadRequest request)
        {
            try
            {
                ProjectReadResponse response = new();

                var projects = dbContext.Project.Where(p => p.RowStatus == (short)RowStatus.Active && p.CustomerId == request.CustomerId)
                                                .OrderBy(p => p.Description)
                                                .ToList();

                // SE MAPEA LA RESPUESTA
                List<CommonObject.Project> projectList = new();
                foreach (var project in projects)
                {
                    CommonObject.Project projectItem = new()
                    {
                        Id = project.Id,
                        Description = project.Description,
                        Customer = project.Customer.Description,
                        Status = project.ProjectStatus.Description
                    };

                    projectList.Add(projectItem);
                }

                // RESPUESTA
                response.Projects = projectList;
                return OperationResult<ProjectReadResponse>.CreateSuccessResult(response);
            }
            catch (Exception ex)
            {
                return OperationResult<ProjectReadResponse>.CreateFailureResult(ex);
            }
        }

        public OperationResult<ProjectReadResponse> GetProjects(ProjectReadRequest request)
        {
            try
            {
                ProjectReadResponse response = new();

                var projectsQuery = dbContext.Project.Where(p => p.RowStatus == (short)RowStatus.Active);

                if (request.EnterpriseId > 0)
                    projectsQuery = projectsQuery.Where(p => p.Customer.EnterpriseId == request.EnterpriseId);

                if (request.CustomerId > 0)
                    projectsQuery = projectsQuery.Where(p => p.CustomerId == request.CustomerId);

                if (request.ProjectStatusId > 0)
                    projectsQuery = projectsQuery.Where(p => p.ProjectStatusId == request.ProjectStatusId);

                var projects = projectsQuery.OrderByDescending(p => p.Id).ToList();

                // SE MAPEA LA RESPUESTA
                List<CommonObject.Project> projectList = new();
                foreach (var project in projects)
                {
                    CommonObject.Project projectItem = new()
                    {
                        Id = project.Id,
                        Description = project.Description,
                        CustomerId = project.CustomerId,
                        Customer = project.Customer.Description,
                        Enterprise = project.Customer.Enterprise.Description,
                        ProjectStatusId = project.ProjectStatusId,
                        Status = project.ProjectStatus.Description,
                        StatusBadgeColor = project.ProjectStatus.BadgeColor,
                        IsClosedStatus = project.ProjectStatus.IsClosed,
                        StartDate = project.StartDate,
                        EndDate = project.EndDate,
                        ActualEndDate = project.ActualEndDate,
                        Created = project.Created.ToString("dd-MM-yyyy"),
                    };

                    projectList.Add(projectItem);
                }

                // RESPUESTA
                response.Projects = projectList;
                return OperationResult<ProjectReadResponse>.CreateSuccessResult(response);
            }
            catch (Exception ex)
            {
                return OperationResult<ProjectReadResponse>.CreateFailureResult(ex);
            }
        }

        public OperationResult<ProjectReadResponse> GetProjectById(long projectId)
        {
            try
            {
                ProjectReadResponse response = new();

                var project = dbContext.Project.FirstOrDefault(p => p.Id == projectId);

                if (project == null)
                    return OperationResult<ProjectReadResponse>.CreateFailureResult("No existe proyecto seleccionado.");

                // SE MAPEA LA RESPUESTA
                List<CommonObject.Project> projectList = new();
                CommonObject.Project projectItem = new()
                {
                    Id = project.Id,
                    Description = project.Description,
                    CustomerId = project.CustomerId,
                    Customer = project.Customer.Description,
                    Enterprise = project.Customer.Enterprise.Description,
                    ProjectStatusId = project.ProjectStatusId,
                    Status = project.ProjectStatus.Description,
                    StatusBadgeColor = project.ProjectStatus.BadgeColor,
                    IsClosedStatus = project.ProjectStatus.IsClosed,
                    StartDate = project.StartDate,
                    EndDate = project.EndDate,
                    ActualEndDate = project.ActualEndDate,
                };

                projectList.Add(projectItem);

                // RESPUESTA
                response.Projects = projectList;
                return OperationResult<ProjectReadResponse>.CreateSuccessResult(response);
            }
            catch (Exception ex)
            {
                return OperationResult<ProjectReadResponse>.CreateFailureResult(ex);
            }
        }
    }
}
