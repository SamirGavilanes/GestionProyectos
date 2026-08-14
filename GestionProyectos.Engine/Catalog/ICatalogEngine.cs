using GestionProyectos.Data.Entities;
using GestionProyectos.Data.Entities.Security;
using GestionProyectos.Data.Entities.TaskManagement;
using GestionProyectos.Engine.Catalog.Request;
using GestionProyectos.Engine.Security.Utilities;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Catalog
{
    public interface ICatalogEngine
    {
        OperationResult<List<User>> GetUsers();
        OperationResult<List<RequirementStatus>> GetRequirementStatuses();
        OperationResult<List<Enterprise>> GetEnterprises();
        OperationResult<List<Customer>> GetCustomers();
        OperationResult<List<Project>> GetProjects();
        OperationResult<List<Requirement>> GetTickets();
        OperationResult<List<Data.Entities.TaskManagement.Requirement>> GetTickets(GetTicketRequest request);
        OperationResult<List<ProjectStatus>> GetProjectStatuses();
        OperationResult<bool> SaveProjectStatus(long id, string description, int order, string badgeColor, bool isClosed, Context context);
        OperationResult<bool> SaveRequirementStatus(long id, string description, int order, string badgeColor, bool isClosed, Context context);
        OperationResult<List<BacklogStatus>> GetBacklogStatuses();
        OperationResult<bool> SaveBacklogStatus(long id, string description, int order, string badgeColor, bool isClosed, Context context);
        OperationResult<List<Priority>> GetPriorities();
        OperationResult<bool> SavePriority(long id, string description, string badgeColor, Context context);
        OperationResult<List<Data.Entities.TaskManagement.TaskStatus>> GetTaskStatuses();
        OperationResult<List<Data.Entities.TaskManagement.TaskBugStatus>> GetTaskBugStatuses();
        OperationResult<List<Data.Entities.TaskManagement.TaskDevelopmentPhase>> GetTaskDevelopmentPhases();

        // MANTENIMIENTO GENÉRICO DE CATÁLOGOS
        OperationResult<List<T>> GetCatalog<T>() where T : AuditBaseEntity, IDescribable, new();
        OperationResult<bool> SaveCatalog<T>(long id, string description, Context context, int? order = null) where T : AuditBaseEntity, IDescribable, new();
        OperationResult<bool> SaveColoredOrderableCatalog<T>(long id, string description, int order, string badgeColor, Context context)
            where T : AuditBaseEntity, IDescribable, IOrderable, IColorable, new();
        OperationResult<bool> SaveHourType(long id, string description, string scope, int order, string badgeColor, Context context);
        OperationResult<bool> DeleteCatalog<T>(long id, Context context) where T : AuditBaseEntity, new();

        // MANTENIMIENTO DE CLIENTES (incluye empresa)
        OperationResult<bool> SaveCustomer(long id, string description, long enterpriseId, Context context);
        OperationResult<bool> DeleteCustomer(long id, Context context);
    }
}
