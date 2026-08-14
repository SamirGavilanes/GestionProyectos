using GestionProyectos.Data;
using GestionProyectos.Engine.Feature.Requirement.Read.Request;
using GestionProyectos.Engine.Feature.Requirement.Read.Response;
using GestionProyectos.Shared.Enums;
using GestionProyectos.Shared.Message;
using Microsoft.EntityFrameworkCore;

namespace GestionProyectos.Engine.Feature.Requirement.Read
{
    public class RequirementReadEngine : IRequirementReadEngine
    {
        private readonly DataDbContext dbContext;

        public RequirementReadEngine(DataDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public OperationResult<RequirementReadResponse> GetTickets() =>
            GetTicketsByEnterpriseProject(new RequirementReadRequest());

        public OperationResult<RequirementReadResponse> GetTicketsByEnterpriseProject(RequirementReadRequest request)
        {
            try
            {
                RequirementReadResponse response = new();

                var ticketsQuery = dbContext.Requirement
                    .Include(t => t.RequirementStatus)
                    .Include(t => t.Priority)
                    .Include(t => t.Project)
                    .Where(t => t.RowStatus == (short)RowStatus.Active);

                if (request.EnterpriseId > 0)
                    ticketsQuery = ticketsQuery.Where(t => t.Project.Customer.EnterpriseId == request.EnterpriseId);

                if (request.CustomerId > 0)
                    ticketsQuery = ticketsQuery.Where(t => t.Project.CustomerId == request.CustomerId);

                if (request.ProjectId > 0)
                    ticketsQuery = ticketsQuery.Where(t => t.ProjectId == request.ProjectId);

                if (request.RequirementStatusId > 0)
                    ticketsQuery = ticketsQuery.Where(t => t.RequirementStatusId == request.RequirementStatusId);

                var tickets = ticketsQuery.OrderByDescending(t => t.Id).ToList();

                response.Tickets = tickets.Select(MapRequirement).ToList();
                return OperationResult<RequirementReadResponse>.CreateSuccessResult(response);
            }
            catch (Exception ex)
            {
                return OperationResult<RequirementReadResponse>.CreateFailureResult(ex);
            }
        }

        public OperationResult<RequirementReadResponse> GetTicket(RequirementReadRequest request)
        {
            try
            {
                RequirementReadResponse response = new();

                var ticket = dbContext.Requirement
                    .Include(t => t.RequirementStatus)
                    .Include(t => t.Priority)
                    .Include(t => t.Project)
                    .Include(t => t.Attachments)
                    .FirstOrDefault(t => t.Id == request.Id);

                if (ticket == null)
                    return OperationResult<RequirementReadResponse>.CreateFailureResult("No existe ticket seleccionado.");

                var ticketItem = MapRequirement(ticket);
                ticketItem.FileAttachments = ticket.Attachments.Select(x => new CommonObject.FileAttachment
                {
                    FileName = x.FileName,
                    FilePath = x.FilePath,
                    Id = x.Id,
                }).ToList();

                response.Tickets.Add(ticketItem);
                return OperationResult<RequirementReadResponse>.CreateSuccessResult(response);
            }
            catch (Exception ex)
            {
                return OperationResult<RequirementReadResponse>.CreateFailureResult(ex);
            }
        }

        private static CommonObject.Requirement MapRequirement(Data.Entities.TaskManagement.Requirement ticket) =>
            new()
            {
                Id = ticket.Id,
                Description = ticket.Description,
                ProjectId = ticket.ProjectId,
                Project = ticket.Project.Description,
                Scope = ticket.Scope,
                RequirementStatusId = ticket.RequirementStatusId,
                RequirementStatus = ticket.RequirementStatus.Description,
                RequirementStatusBadgeColor = ticket.RequirementStatus.BadgeColor,
                PriorityId = ticket.PriorityId,
                Priority = ticket.Priority.Description,
                PriorityBadgeColor = ticket.Priority.BadgeColor,
                StartDate = ticket.StartDate,
                EndDate = ticket.EndDate,
                ActualEndDate = ticket.ActualEndDate,
                RequesterName = ticket.RequesterName,
                RequestDate = ticket.RequestDate,
                ImpactedSystems = ticket.ImpactedSystems,
                FreshDeskTicketNumber = ticket.FreshDeskTicketNumber,
                IsWithinOriginalScope = ticket.IsWithinOriginalScope,
                ScopeChangeReason = ticket.ScopeChangeReason,
                IsProductionReprocess = ticket.IsProductionReprocess,
                RowStatus = ticket.RowStatus
            };
    }
}
