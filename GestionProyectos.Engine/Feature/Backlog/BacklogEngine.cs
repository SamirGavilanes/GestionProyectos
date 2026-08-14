using GestionProyectos.Data;
using GestionProyectos.Data.Entities.TaskManagement;
using GestionProyectos.Engine.Feature.Backlog.Request;
using GestionProyectos.Engine.Feature.Backlog.Response;
using GestionProyectos.Shared.Enums;
using GestionProyectos.Shared.Message;
using Microsoft.EntityFrameworkCore;

namespace GestionProyectos.Engine.Feature.Backlog
{
    public class BacklogEngine : IBacklogEngine
    {
        private readonly DataDbContext dbContext;

        public BacklogEngine(DataDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public OperationResult<BacklogListResponse> GetItems(BacklogListRequest request)
        {
            try
            {
                var items = dbContext.BacklogItem
                    .AsNoTracking()
                    .Where(x => x.RowStatus == (short)RowStatus.Active)
                    .OrderByDescending(x => x.Id)
                    .Select(x => new BacklogItemResponse
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        BacklogStatusId = x.BacklogStatusId,
                        CustomerId = x.CustomerId ?? 0,
                        EnterpriseId = x.Customer != null ? x.Customer.EnterpriseId : 0,
                        Customer = x.Customer != null ? x.Customer.Description : string.Empty,
                        Enterprise = x.Customer != null && x.Customer.Enterprise != null
                            ? x.Customer.Enterprise.Description
                            : string.Empty,
                        Status = x.BacklogStatus.Description,
                        StatusBadgeColor = x.BacklogStatus.BadgeColor,
                        StatusIsClosed = x.BacklogStatus.IsClosed
                    })
                    .ToList();

                return OperationResult<BacklogListResponse>.CreateSuccessResult(new BacklogListResponse
                {
                    Items = items
                });
            }
            catch (Exception ex)
            {
                return OperationResult<BacklogListResponse>.CreateFailureResult(ex);
            }
        }

        public OperationResult<BacklogSaveResponse> Save(BacklogSaveRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                    return OperationResult<BacklogSaveResponse>.CreateFailureResult("La descripción es obligatoria.");

                if (request.Id == 0 && request.CustomerId <= 0)
                    return OperationResult<BacklogSaveResponse>.CreateFailureResult("Seleccione un cliente.");

                if (request.CustomerId > 0 &&
                    !dbContext.Customer.Any(c => c.Id == request.CustomerId && c.RowStatus == (short)RowStatus.Active))
                    return OperationResult<BacklogSaveResponse>.CreateFailureResult("El cliente no es válido.");

                var statusId = request.BacklogStatusId;
                if (statusId <= 0)
                    statusId = ResolveDefaultStatusId();

                var status = dbContext.BacklogStatus.FirstOrDefault(x =>
                    x.Id == statusId && x.RowStatus == (short)RowStatus.Active);
                if (status == null)
                    return OperationResult<BacklogSaveResponse>.CreateFailureResult("El estado no es válido.");

                long savedId;
                if (request.Id == 0)
                {
                    var entity = new BacklogItem
                    {
                        Id = dbContext.BacklogItem.Any() ? dbContext.BacklogItem.Max(x => x.Id) + 1 : 1,
                        Name = request.Name.Trim(),
                        Description = request.Description?.Trim() ?? string.Empty,
                        BacklogStatusId = status.Id,
                        CustomerId = request.CustomerId,
                        RowStatus = (short)RowStatus.Active,
                        Created = DateTime.UtcNow,
                        CreatedBy = request.Context.UserId
                    };
                    dbContext.BacklogItem.Add(entity);
                    dbContext.SaveChanges();
                    savedId = entity.Id;
                }
                else
                {
                    var entity = dbContext.BacklogItem.FirstOrDefault(x =>
                        x.Id == request.Id && x.RowStatus == (short)RowStatus.Active);
                    if (entity == null)
                        return OperationResult<BacklogSaveResponse>.CreateFailureResult("No se encontró el registro.");

                    entity.Name = request.Name.Trim();
                    entity.Description = request.Description?.Trim() ?? string.Empty;
                    entity.BacklogStatusId = status.Id;
                    if (request.CustomerId > 0)
                        entity.CustomerId = request.CustomerId;
                    entity.Updated = DateTime.UtcNow;
                    entity.UpdatedBy = request.Context.UserId;
                    dbContext.SaveChanges();
                    savedId = entity.Id;
                }

                return OperationResult<BacklogSaveResponse>.CreateSuccessResult(new BacklogSaveResponse { Id = savedId });
            }
            catch (Exception ex)
            {
                return OperationResult<BacklogSaveResponse>.CreateFailureResult(ex);
            }
        }

        public OperationResult<bool> Delete(BacklogDeleteRequest request)
        {
            try
            {
                var entity = dbContext.BacklogItem.FirstOrDefault(x =>
                    x.Id == request.Id && x.RowStatus == (short)RowStatus.Active);
                if (entity == null)
                    return OperationResult<bool>.CreateFailureResult("No se encontró el registro.");

                entity.RowStatus = (short)RowStatus.Inactive;
                entity.Updated = DateTime.UtcNow;
                entity.UpdatedBy = request.Context.UserId;
                dbContext.SaveChanges();
                return OperationResult<bool>.CreateSuccessResult(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.CreateFailureResult(ex);
            }
        }

        private long ResolveDefaultStatusId()
        {
            var statuses = dbContext.BacklogStatus
                .Where(x => x.RowStatus == (short)RowStatus.Active)
                .OrderBy(x => x.Order)
                .ToList();

            return statuses.FirstOrDefault(x =>
                       x.Description.Equals("Pendiente", StringComparison.OrdinalIgnoreCase))?.Id
                   ?? statuses.FirstOrDefault(x => !x.IsClosed)?.Id
                   ?? 0;
        }
    }
}
