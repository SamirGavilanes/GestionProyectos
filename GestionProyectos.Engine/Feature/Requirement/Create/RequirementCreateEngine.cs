using GestionProyectos.Data;
using GestionProyectos.Data.Entities.TaskManagement;
using GestionProyectos.Engine.Feature.Requirement.Create.Request;
using GestionProyectos.Engine.Feature.Requirement.Create.Response;
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

namespace GestionProyectos.Engine.Feature.Requirement.Create
{
    public class RequirementCreateEngine : IRequirementCreateEngine
    {
        private readonly DataDbContext dbContext;
        private readonly IS3UploadFileEngine s3UploadFileEngine;
        private readonly IOptions<AppSettingsManagerBase> appSettings;
        private readonly ISendEmailEngine sendEmailEngine;
        public RequirementCreateEngine(DataDbContext dbContext,
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
        public OperationResult<RequirementCreateResponse> Execute(RequirementCreateRequest request)
        {
            try
            {
                var status = dbContext.RequirementStatus
                    .FirstOrDefault(s => s.Id == request.RequirementStatusId && s.RowStatus == (short)RowStatus.Active);
                if (status != null &&
                    FinalizeStatusHelper.IsRequirementFinalized(status.IsClosed) &&
                    !PermissionHelper.CanFinalizeRequirement(request.Context))
                    return OperationResult<RequirementCreateResponse>.CreateFailureResult("No tiene permiso para finalizar requerimientos.");

                if (!request.IsWithinOriginalScope && !IsValidScopeChangeReason(request.ScopeChangeReason))
                    return OperationResult<RequirementCreateResponse>.CreateFailureResult("Indique si el cambio de alcance es por nosotros o externo.");

                RequirementCreateResponse response = new();

                #region GUARDAR TICKET
                // PETICION PARA GUARDAR TICKET EN BASE DE DATOS
                Data.Entities.TaskManagement.Requirement ticket = new()
                {
                    Description = request.Description,
                    ProjectId = request.ProjectId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    ActualEndDate = request.ActualEndDate,
                    RequesterName = request.RequesterName,
                    RequestDate = request.RequestDate,
                    ImpactedSystems = request.ImpactedSystems,
                    FreshDeskTicketNumber = string.IsNullOrWhiteSpace(request.FreshDeskTicketNumber)
                        ? null
                        : request.FreshDeskTicketNumber.Trim(),
                    Scope = request.Scope,
                    PriorityId = request.PriorityId,
                    IsWithinOriginalScope = request.IsWithinOriginalScope,
                    ScopeChangeReason = request.IsWithinOriginalScope ? null : request.ScopeChangeReason,
                    IsProductionReprocess = request.IsProductionReprocess,
                    RowStatus = (short)RowStatus.Active,
                    Created = DateTime.UtcNow,
                    CreatedBy = request.Context.UserId,
                    RequirementStatusId = request.RequirementStatusId
                };

                // SE GUARDAR TICKET
                dbContext.Requirement.Add(ticket);
                dbContext.SaveChanges();
                #endregion

                #region GUARDAR ADJUNTOS EN S3
                if (request.Files.Count > 0)
                {
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

                return OperationResult<RequirementCreateResponse>.CreateSuccessResult(response);
            }
            catch (Exception ex)
            {
                return OperationResult<RequirementCreateResponse>.CreateFailureResult(ex);
            }
        }

        private static bool IsValidScopeChangeReason(short? reason) =>
            reason == (short)TaskScopeChangeReason.Internal || reason == (short)TaskScopeChangeReason.External;
    }
}
