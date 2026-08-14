using GestionProyectos.Data;
using GestionProyectos.Data.Entities;
using GestionProyectos.Data.Entities.Security;
using GestionProyectos.Data.Entities.TaskManagement;
using GestionProyectos.Engine.Catalog.Request;
using GestionProyectos.Engine.Security.Utilities;
using GestionProyectos.Shared.Enums;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Catalog
{
    public class CatalogEngine : ICatalogEngine
    {
        private readonly DataDbContext dbContext;

        public CatalogEngine(DataDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public OperationResult<List<Customer>> GetCustomers()
        {
            try
            {
                List<Customer> customers = dbContext.Customer.Where(u => u.RowStatus == (short)RowStatus.Active)
                                                             .OrderBy(o => o.Description).ToList();


                return OperationResult<List<Customer>>.CreateSuccessResult(customers);
            }
            catch (Exception ex)
            {
                return OperationResult<List<Customer>>.CreateFailureResult(ex);
            }
        }

        public OperationResult<List<Enterprise>> GetEnterprises()
        {
            try
            {
                List<Enterprise> enterprises = dbContext.Enterprise.Where(u => u.RowStatus == (short)RowStatus.Active)
                                                                   .OrderBy(o => o.Description).ToList();

                return OperationResult<List<Enterprise>>.CreateSuccessResult(enterprises);
            }
            catch (Exception ex)
            {
                return OperationResult<List<Enterprise>>.CreateFailureResult(ex);
            }
        }

        public OperationResult<List<Project>> GetProjects()
        {
            try
            {
                List<Project> projects = dbContext.Project.Where(u => u.RowStatus == (short)RowStatus.Active)
                                                          .OrderBy(o => o.Description).ToList(); ;

                return OperationResult<List<Project>>.CreateSuccessResult(projects);
            }
            catch (Exception ex)
            {
                return OperationResult<List<Project>>.CreateFailureResult(ex);
            }
        }

        public OperationResult<List<ProjectStatus>> GetProjectStatuses()
        {
            try
            {
                List<ProjectStatus> projects = dbContext.ProjectStatus.Where(u => u.RowStatus == (short)RowStatus.Active)
                                                                      .OrderBy(o => o.Order).ThenBy(o => o.Description).ToList();

                return OperationResult<List<ProjectStatus>>.CreateSuccessResult(projects);
            }
            catch (Exception ex)
            {
                return OperationResult<List<ProjectStatus>>.CreateFailureResult(ex);
            }
        }

        public OperationResult<bool> SaveProjectStatus(long id, string description, int order, string badgeColor, bool isClosed, Context context)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(description))
                    return OperationResult<bool>.CreateFailureResult("La descripción es obligatoria.");

                if (id == 0)
                {
                    dbContext.ProjectStatus.Add(new ProjectStatus
                    {
                        Description = description.Trim(),
                        Order = order > 0 ? order : GetNextOrder<ProjectStatus>(),
                        BadgeColor = string.IsNullOrWhiteSpace(badgeColor) ? "gray" : badgeColor.Trim(),
                        IsClosed = isClosed,
                        RowStatus = (short)RowStatus.Active,
                        Created = DateTime.UtcNow,
                        CreatedBy = context.UserId
                    });
                }
                else
                {
                    var entity = dbContext.ProjectStatus.FirstOrDefault(x => x.Id == id);
                    if (entity == null)
                        return OperationResult<bool>.CreateFailureResult("No se encontró el registro.");

                    entity.Description = description.Trim();
                    entity.Order = order;
                    entity.BadgeColor = string.IsNullOrWhiteSpace(badgeColor) ? "gray" : badgeColor.Trim();
                    entity.IsClosed = isClosed;
                    entity.Updated = DateTime.UtcNow;
                    entity.UpdatedBy = context.UserId;
                }

                dbContext.SaveChanges();
                return OperationResult<bool>.CreateSuccessResult(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.CreateFailureResult(ex);
            }
        }

        public OperationResult<List<RequirementStatus>> GetRequirementStatuses()
        {
            try
            {
                List<RequirementStatus> requirementStatus = dbContext.RequirementStatus.Where(u => u.RowStatus == (short)RowStatus.Active)
                                                                                       .OrderBy(o => o.Order).ThenBy(o => o.Description).ToList();

                return OperationResult<List<RequirementStatus>>.CreateSuccessResult(requirementStatus);
            }
            catch (Exception ex)
            {
                return OperationResult<List<RequirementStatus>>.CreateFailureResult(ex);
            }
        }

        public OperationResult<bool> SaveRequirementStatus(long id, string description, int order, string badgeColor, bool isClosed, Context context)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(description))
                    return OperationResult<bool>.CreateFailureResult("La descripción es obligatoria.");

                if (id == 0)
                {
                    dbContext.RequirementStatus.Add(new RequirementStatus
                    {
                        Description = description.Trim(),
                        Order = order > 0 ? order : GetNextOrder<RequirementStatus>(),
                        BadgeColor = string.IsNullOrWhiteSpace(badgeColor) ? "gray" : badgeColor.Trim(),
                        IsClosed = isClosed,
                        RowStatus = (short)RowStatus.Active,
                        Created = DateTime.UtcNow,
                        CreatedBy = context.UserId
                    });
                }
                else
                {
                    var entity = dbContext.RequirementStatus.FirstOrDefault(x => x.Id == id);
                    if (entity == null)
                        return OperationResult<bool>.CreateFailureResult("No se encontró el registro.");

                    entity.Description = description.Trim();
                    entity.Order = order;
                    entity.BadgeColor = string.IsNullOrWhiteSpace(badgeColor) ? "gray" : badgeColor.Trim();
                    entity.IsClosed = isClosed;
                    entity.Updated = DateTime.UtcNow;
                    entity.UpdatedBy = context.UserId;
                }

                dbContext.SaveChanges();
                return OperationResult<bool>.CreateSuccessResult(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.CreateFailureResult(ex);
            }
        }

        public OperationResult<List<BacklogStatus>> GetBacklogStatuses()
        {
            try
            {
                var items = dbContext.BacklogStatus
                    .Where(u => u.RowStatus == (short)RowStatus.Active)
                    .OrderBy(o => o.Order)
                    .ThenBy(o => o.Description)
                    .ToList();

                return OperationResult<List<BacklogStatus>>.CreateSuccessResult(items);
            }
            catch (Exception ex)
            {
                return OperationResult<List<BacklogStatus>>.CreateFailureResult(ex);
            }
        }

        public OperationResult<bool> SaveBacklogStatus(long id, string description, int order, string badgeColor, bool isClosed, Context context)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(description))
                    return OperationResult<bool>.CreateFailureResult("La descripción es obligatoria.");

                if (id == 0)
                {
                    dbContext.BacklogStatus.Add(new BacklogStatus
                    {
                        Description = description.Trim(),
                        Order = order > 0 ? order : GetNextOrder<BacklogStatus>(),
                        BadgeColor = string.IsNullOrWhiteSpace(badgeColor) ? "gray" : badgeColor.Trim(),
                        IsClosed = isClosed,
                        RowStatus = (short)RowStatus.Active,
                        Created = DateTime.UtcNow,
                        CreatedBy = context.UserId
                    });
                }
                else
                {
                    var entity = dbContext.BacklogStatus.FirstOrDefault(x => x.Id == id);
                    if (entity == null)
                        return OperationResult<bool>.CreateFailureResult("No se encontró el registro.");

                    entity.Description = description.Trim();
                    entity.Order = order;
                    entity.BadgeColor = string.IsNullOrWhiteSpace(badgeColor) ? "gray" : badgeColor.Trim();
                    entity.IsClosed = isClosed;
                    entity.Updated = DateTime.UtcNow;
                    entity.UpdatedBy = context.UserId;
                }

                dbContext.SaveChanges();
                return OperationResult<bool>.CreateSuccessResult(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.CreateFailureResult(ex);
            }
        }

        public OperationResult<List<Requirement>> GetTickets()
        {
            try
            {
                List<Requirement> tickets = dbContext.Requirement.Where(u => u.RowStatus == (short)RowStatus.Active)
                                                                 .OrderBy(o => o.Description).ToList();

                return OperationResult<List<Requirement>>.CreateSuccessResult(tickets);
            }
            catch (Exception ex)
            {
                return OperationResult<List<Requirement>>.CreateFailureResult(ex);
            }
        }

        public OperationResult<List<Data.Entities.TaskManagement.Requirement>> GetTickets(GetTicketRequest request)
        {
            try
            {
                List<Data.Entities.TaskManagement.Requirement> tickets = dbContext.Requirement.Where(u => u.RowStatus == (short)RowStatus.Active && u.ProjectId == request.ProjectId)
                                                                                              .OrderBy(o => o.Description).ToList();

                return OperationResult<List<Data.Entities.TaskManagement.Requirement>>.CreateSuccessResult(tickets);
            }
            catch (Exception ex)
            {
                return OperationResult<List<Data.Entities.TaskManagement.Requirement>>.CreateFailureResult(ex);
            }
        }

        public OperationResult<List<User>> GetUsers()
        {
            try
            {
                List<User> users = dbContext.User.Where(u => u.RowStatus == (short)RowStatus.Active)
                                                 .OrderBy(o => o.Name).ToList();

                return OperationResult<List<User>>.CreateSuccessResult(users);
            }
            catch (Exception ex)
            {
                return OperationResult<List<User>>.CreateFailureResult(ex);
            }
        }

        public OperationResult<List<Priority>> GetPriorities()
        {
            try
            {
                List<Priority> priorities = dbContext.Priority.Where(u => u.RowStatus == (short)RowStatus.Active)
                                                              .OrderBy(o => o.Description).ToList();

                return OperationResult<List<Priority>>.CreateSuccessResult(priorities);
            }
            catch (Exception ex)
            {
                return OperationResult<List<Priority>>.CreateFailureResult(ex);
            }
        }

        public OperationResult<bool> SavePriority(long id, string description, string badgeColor, Context context)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(description))
                    return OperationResult<bool>.CreateFailureResult("La descripción es obligatoria.");

                if (id == 0)
                {
                    dbContext.Priority.Add(new Priority
                    {
                        Description = description.Trim(),
                        BadgeColor = string.IsNullOrWhiteSpace(badgeColor) ? "gray" : badgeColor.Trim(),
                        RowStatus = (short)RowStatus.Active,
                        Created = DateTime.UtcNow,
                        CreatedBy = context.UserId
                    });
                }
                else
                {
                    var entity = dbContext.Priority.FirstOrDefault(x => x.Id == id);
                    if (entity == null)
                        return OperationResult<bool>.CreateFailureResult("No se encontró el registro.");

                    entity.Description = description.Trim();
                    entity.BadgeColor = string.IsNullOrWhiteSpace(badgeColor) ? "gray" : badgeColor.Trim();
                    entity.Updated = DateTime.UtcNow;
                    entity.UpdatedBy = context.UserId;
                }

                dbContext.SaveChanges();
                return OperationResult<bool>.CreateSuccessResult(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.CreateFailureResult(ex);
            }
        }

        public OperationResult<List<Data.Entities.TaskManagement.TaskStatus>> GetTaskStatuses()
        {
            try
            {
                List<Data.Entities.TaskManagement.TaskStatus> taskStatuses = dbContext.TaskStatus.Where(u => u.RowStatus == (short)RowStatus.Active)
                                                                    .OrderBy(o => o.Order).ThenBy(o => o.Description).ToList();

                return OperationResult<List<Data.Entities.TaskManagement.TaskStatus>>.CreateSuccessResult(taskStatuses);
            }
            catch (Exception ex)
            {
                return OperationResult<List<Data.Entities.TaskManagement.TaskStatus>>.CreateFailureResult(ex);
            }
        }

        public OperationResult<List<Data.Entities.TaskManagement.TaskBugStatus>> GetTaskBugStatuses()
        {
            try
            {
                var statuses = dbContext.TaskBugStatus
                    .Where(s => s.RowStatus == (short)RowStatus.Active)
                    .OrderBy(s => s.Order)
                    .ThenBy(s => s.Description)
                    .ToList();

                return OperationResult<List<Data.Entities.TaskManagement.TaskBugStatus>>.CreateSuccessResult(statuses);
            }
            catch (Exception ex)
            {
                return OperationResult<List<Data.Entities.TaskManagement.TaskBugStatus>>.CreateFailureResult(ex);
            }
        }

        public OperationResult<List<Data.Entities.TaskManagement.TaskDevelopmentPhase>> GetTaskDevelopmentPhases()
        {
            try
            {
                var phases = dbContext.TaskDevelopmentPhase
                    .Where(p => p.RowStatus == (short)RowStatus.Active)
                    .OrderBy(p => p.Order)
                    .ThenBy(p => p.Description)
                    .ToList();

                return OperationResult<List<Data.Entities.TaskManagement.TaskDevelopmentPhase>>.CreateSuccessResult(phases);
            }
            catch (Exception ex)
            {
                return OperationResult<List<Data.Entities.TaskManagement.TaskDevelopmentPhase>>.CreateFailureResult(ex);
            }
        }

        #region MANTENIMIENTO GENÉRICO DE CATÁLOGOS
        public OperationResult<List<T>> GetCatalog<T>() where T : AuditBaseEntity, IDescribable, new()
        {
            try
            {
                var items = dbContext.Set<T>().Where(x => x.RowStatus == (short)RowStatus.Active).ToList();

                if (typeof(IOrderable).IsAssignableFrom(typeof(T)))
                {
                    items = items.OrderBy(x => ((IOrderable)x).Order).ThenBy(x => x.Description).ToList();
                }
                else
                {
                    items = items.OrderBy(x => x.Description).ToList();
                }

                return OperationResult<List<T>>.CreateSuccessResult(items);
            }
            catch (Exception ex)
            {
                return OperationResult<List<T>>.CreateFailureResult(ex);
            }
        }

        public OperationResult<bool> SaveCatalog<T>(long id, string description, Context context, int? order = null) where T : AuditBaseEntity, IDescribable, new()
        {
            return SaveCatalogInternal<T>(id, description, order, context);
        }

        public OperationResult<bool> SaveColoredOrderableCatalog<T>(long id, string description, int order, string badgeColor, Context context)
            where T : AuditBaseEntity, IDescribable, IOrderable, IColorable, new()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(description))
                    return OperationResult<bool>.CreateFailureResult("La descripción es obligatoria.");

                var color = string.IsNullOrWhiteSpace(badgeColor) ? "gray" : badgeColor.Trim();

                if (id == 0)
                {
                    T entity = new()
                    {
                        Description = description.Trim(),
                        Order = order > 0 ? order : GetNextOrder<T>(),
                        BadgeColor = color,
                        RowStatus = (short)RowStatus.Active,
                        Created = DateTime.UtcNow,
                        CreatedBy = context.UserId
                    };
                    dbContext.Set<T>().Add(entity);
                }
                else
                {
                    var entity = dbContext.Set<T>().FirstOrDefault(x => x.Id == id);
                    if (entity == null)
                        return OperationResult<bool>.CreateFailureResult("No se encontró el registro.");

                    entity.Description = description.Trim();
                    entity.Order = order;
                    entity.BadgeColor = color;
                    entity.Updated = DateTime.UtcNow;
                    entity.UpdatedBy = context.UserId;
                }

                dbContext.SaveChanges();
                return OperationResult<bool>.CreateSuccessResult(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.CreateFailureResult(ex);
            }
        }

        public OperationResult<bool> SaveHourType(long id, string description, string scope, int order, string badgeColor, Context context)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(description))
                    return OperationResult<bool>.CreateFailureResult("La descripción es obligatoria.");

                var color = string.IsNullOrWhiteSpace(badgeColor) ? "gray" : badgeColor.Trim();
                var scopeValue = scope?.Trim() ?? string.Empty;

                if (id == 0)
                {
                    var entity = new HourType
                    {
                        Description = description.Trim(),
                        Scope = scopeValue,
                        Order = order > 0 ? order : GetNextOrder<HourType>(),
                        BadgeColor = color,
                        RowStatus = (short)RowStatus.Active,
                        Created = DateTime.UtcNow,
                        CreatedBy = context.UserId
                    };
                    dbContext.HourType.Add(entity);
                }
                else
                {
                    var entity = dbContext.HourType.FirstOrDefault(x => x.Id == id);
                    if (entity == null)
                        return OperationResult<bool>.CreateFailureResult("No se encontró el registro.");

                    entity.Description = description.Trim();
                    entity.Scope = scopeValue;
                    entity.Order = order;
                    entity.BadgeColor = color;
                    entity.Updated = DateTime.UtcNow;
                    entity.UpdatedBy = context.UserId;
                }

                dbContext.SaveChanges();
                return OperationResult<bool>.CreateSuccessResult(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.CreateFailureResult(ex);
            }
        }

        private OperationResult<bool> SaveCatalogInternal<T>(long id, string description, int? order, Context context) where T : AuditBaseEntity, IDescribable, new()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(description))
                    return OperationResult<bool>.CreateFailureResult("La descripción es obligatoria.");

                if (id == 0)
                {
                    T entity = new()
                    {
                        Description = description.Trim(),
                        RowStatus = (short)RowStatus.Active,
                        Created = DateTime.UtcNow,
                        CreatedBy = context.UserId
                    };

                    if (entity is IOrderable orderable)
                    {
                        orderable.Order = order > 0
                            ? order.Value
                            : GetNextOrder<T>();
                    }

                    dbContext.Set<T>().Add(entity);
                }
                else
                {
                    T? entity = dbContext.Set<T>().FirstOrDefault(x => x.Id == id);
                    if (entity == null)
                        return OperationResult<bool>.CreateFailureResult("No se encontró el registro.");

                    entity.Description = description.Trim();
                    if (entity is IOrderable orderable && order.HasValue)
                        orderable.Order = order.Value;

                    entity.Updated = DateTime.UtcNow;
                    entity.UpdatedBy = context.UserId;
                }

                dbContext.SaveChanges();
                return OperationResult<bool>.CreateSuccessResult(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.CreateFailureResult(ex);
            }
        }

        private int GetNextOrder<T>() where T : AuditBaseEntity, new()
        {
            var maxOrder = dbContext.Set<T>()
                .Where(x => x.RowStatus == (short)RowStatus.Active)
                .AsEnumerable()
                .OfType<IOrderable>()
                .Select(x => x.Order)
                .DefaultIfEmpty(0)
                .Max();
            return maxOrder + 1;
        }

        public OperationResult<bool> DeleteCatalog<T>(long id, Context context) where T : AuditBaseEntity, new()
        {
            try
            {
                T? entity = dbContext.Set<T>().FirstOrDefault(x => x.Id == id);
                if (entity == null)
                    return OperationResult<bool>.CreateFailureResult("No se encontró el registro.");

                entity.RowStatus = (short)RowStatus.Inactive;
                entity.Updated = DateTime.UtcNow;
                entity.UpdatedBy = context.UserId;

                dbContext.SaveChanges();
                return OperationResult<bool>.CreateSuccessResult(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.CreateFailureResult(ex);
            }
        }
        #endregion

        #region MANTENIMIENTO DE CLIENTES
        public OperationResult<bool> SaveCustomer(long id, string description, long enterpriseId, Context context)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(description))
                    return OperationResult<bool>.CreateFailureResult("La descripción es obligatoria.");

                if (enterpriseId == 0)
                    return OperationResult<bool>.CreateFailureResult("Debe seleccionar una empresa.");

                if (id == 0)
                {
                    Customer customer = new()
                    {
                        Description = description.Trim(),
                        EnterpriseId = enterpriseId,
                        RowStatus = (short)RowStatus.Active,
                        Created = DateTime.UtcNow,
                        CreatedBy = context.UserId
                    };
                    dbContext.Customer.Add(customer);
                }
                else
                {
                    Customer? customer = dbContext.Customer.FirstOrDefault(x => x.Id == id);
                    if (customer == null)
                        return OperationResult<bool>.CreateFailureResult("No se encontró el cliente.");

                    customer.Description = description.Trim();
                    customer.EnterpriseId = enterpriseId;
                    customer.Updated = DateTime.UtcNow;
                    customer.UpdatedBy = context.UserId;
                }

                dbContext.SaveChanges();
                return OperationResult<bool>.CreateSuccessResult(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.CreateFailureResult(ex);
            }
        }

        public OperationResult<bool> DeleteCustomer(long id, Context context)
        {
            try
            {
                Customer? customer = dbContext.Customer.FirstOrDefault(x => x.Id == id);
                if (customer == null)
                    return OperationResult<bool>.CreateFailureResult("No se encontró el cliente.");

                customer.RowStatus = (short)RowStatus.Inactive;
                customer.Updated = DateTime.UtcNow;
                customer.UpdatedBy = context.UserId;

                dbContext.SaveChanges();
                return OperationResult<bool>.CreateSuccessResult(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.CreateFailureResult(ex);
            }
        }
        #endregion
    }
}
