using GestionProyectos.Data;
using GestionProyectos.Data.Entities.TaskManagement;
using GestionProyectos.Engine.Feature.Requirement.Update.Request;
using GestionProyectos.Engine.Feature.Requirement.Update.Response;
using GestionProyectos.Engine.Feature;
using GestionProyectos.Engine.Security.Utilities;
using GestionProyectos.Engine.Utility.S3UploadFile;
using GestionProyectos.Engine.Utility.S3UploadFile.Request;
using GestionProyectos.Engine.Utility.SendEmail;
using GestionProyectos.Engine.Utility.SendEmail.Request;
using GestionProyectos.Shared.Configurations;
using GestionProyectos.Shared.Enums;
using GestionProyectos.Shared.Message;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace GestionProyectos.Engine.Feature.Requirement.Update
{
    public class RequirementUpdateEngine : IRequirementUpdateEngine
    {
        private readonly DataDbContext dbContext;
        private readonly IS3UploadFileEngine s3UploadFileEngine;
        private readonly IOptions<AppSettingsManagerBase> appSettings;
        private readonly ISendEmailEngine sendEmailEngine;
        public RequirementUpdateEngine(DataDbContext dbContext,
            IS3UploadFileEngine s3UploadFileEngine,
            IOptions<AppSettingsManagerBase> appSettings,
            ISendEmailEngine sendEmailEngine
            )
        {
            this.dbContext = dbContext;
            this.s3UploadFileEngine = s3UploadFileEngine;
            this.appSettings = appSettings;
            this.sendEmailEngine = sendEmailEngine;
        }
        public OperationResult<RequirementUpdateResponse> Execute(RequirementUpdateRequest request)
        {
            try
            {
                // BUSCAR TICKET
                var ticket = dbContext.Requirement.FirstOrDefault(t => t.Id == request.Id);

                // VALIDAR QUE EXISTE TICKET
                if (ticket == null)
                    return OperationResult<RequirementUpdateResponse>.CreateFailureResult("No existe el ticket que se desea actualizar.");

                if (request.RequirementStatusId != ticket.RequirementStatusId)
                {
                    var newStatus = dbContext.RequirementStatus
                        .FirstOrDefault(s => s.Id == request.RequirementStatusId && s.RowStatus == (short)RowStatus.Active);
                    if (newStatus != null &&
                        FinalizeStatusHelper.IsRequirementFinalized(newStatus.IsClosed) &&
                        !PermissionHelper.CanFinalizeRequirement(request.Context))
                        return OperationResult<RequirementUpdateResponse>.CreateFailureResult("No tiene permiso para finalizar requerimientos.");
                }

                if (!request.IsWithinOriginalScope && !IsValidScopeChangeReason(request.ScopeChangeReason))
                    return OperationResult<RequirementUpdateResponse>.CreateFailureResult("Indique si el cambio de alcance es por nosotros o externo.");

                #region EDITAR TICKET
                // PETICION PARA GUARDAR TICKET EN BASE DE DATOS
                ticket.Description = request.Description;
                ticket.ProjectId = request.ProjectId;
                ticket.StartDate = request.StartDate;
                ticket.EndDate = request.EndDate;
                ticket.ActualEndDate = request.ActualEndDate;
                ticket.RequesterName = request.RequesterName;
                ticket.RequestDate = request.RequestDate;
                ticket.ImpactedSystems = request.ImpactedSystems;
                ticket.FreshDeskTicketNumber = string.IsNullOrWhiteSpace(request.FreshDeskTicketNumber)
                    ? null
                    : request.FreshDeskTicketNumber.Trim();
                ticket.Scope = request.Scope;
                ticket.PriorityId = request.PriorityId;
                ticket.IsWithinOriginalScope = request.IsWithinOriginalScope;
                ticket.ScopeChangeReason = request.IsWithinOriginalScope ? null : request.ScopeChangeReason;
                ticket.IsProductionReprocess = request.IsProductionReprocess;
                ticket.Updated = DateTime.UtcNow;
                ticket.UpdatedBy = request.Context.UserId;
                ticket.RequirementStatusId = request.RequirementStatusId;

                // SE GUARDAR TICKET
                dbContext.Requirement.Update(ticket);
                dbContext.SaveChanges();
                #endregion

                #region GUARDAR ADJUNTOS EN S3
                if (request.Files.Count > 0)
                {
                    // ELIMINAR DOCUMENTOS ADJUNTOS
                    //var ticketAttachments = dbContext.Attachment.Where(a => a.RequirementId == request.ProjectId);
                    //dbContext.Attachment.RemoveRange(ticketAttachments);
                    //dbContext.SaveChanges();

                    foreach (var file in request.Files)
                    {
                        // SE CREA PETICION PARA GUARDADO EN S3
                        S3UploadFileRequest s3UploadFileRequest = new()
                        {
                            AccessKey = appSettings.Value.Configurations.S3Config.AccessKey,
                            BuketName = appSettings.Value.Configurations.S3Config.BuketName,
                            SecretAccessKey = appSettings.Value.Configurations.S3Config.SecretAccessKey,
                            DestinationPath = $"{appSettings.Value.Configurations.S3Config.DestinationPath}/{ticket.ProjectId}/{ticket.Id}/",
                            Active = appSettings.Value.Configurations.S3Config.Active,
                            Name = file.Name,
                            File = file.File
                        };
                        s3UploadFileEngine.Execute(s3UploadFileRequest);

                        // OBJETO PAR AGUARDAR REGISTRO DE DOCUMENTOS ADJUNTOS
                        Attachment attachment = new()
                        {
                            FileName = file.Name,
                            FilePath = $"{appSettings.Value.Configurations.S3Config.DestinationPath}/{ticket.ProjectId}/{ticket.Id}/",
                            RequirementId = ticket.Id,
                            RowStatus = (short)RowStatus.Active,
                            Created = DateTime.UtcNow,
                            CreatedBy = request.Context.UserId
                        };
                        dbContext.Attachment.Add(attachment);
                        dbContext.SaveChanges();
                    }
                }
                #endregion


                return OperationResult<RequirementUpdateResponse>.CreateSuccessResult(new RequirementUpdateResponse());
            }
            catch (Exception ex)
            {
                return OperationResult<RequirementUpdateResponse>.CreateFailureResult(ex);
            }
        }

        private static bool IsValidScopeChangeReason(short? reason) =>
            reason == (short)TaskScopeChangeReason.Internal || reason == (short)TaskScopeChangeReason.External;
    }
}
