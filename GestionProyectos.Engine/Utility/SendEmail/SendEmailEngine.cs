using GestionProyectos.Data;
using GestionProyectos.Engine.Utility.SendEmail.Request;
using GestionProyectos.Engine.Utility.SendEmail.Response;
using GestionProyectos.Shared.Configurations;
using GestionProyectos.Shared.Message;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Text;

namespace GestionProyectos.Engine.Utility.SendEmail
{
    public class SendEmailEngine : ISendEmailEngine
    {
        private readonly DataDbContext dbContext;
        private readonly IOptions<AppSettingsManagerBase> appSettings;
        public SendEmailEngine(DataDbContext dbContext,
            IOptions<AppSettingsManagerBase> appSettings)
        {
            this.dbContext = dbContext;
            this.appSettings = appSettings;
        }
        public OperationResult<SendEmailResponse> Execute(SendEmailRequest request)
        {
            try
            {
                SendEmailResponse response = new();

                Task<bool> result = Task.FromResult(false);
                StringBuilder bodyText = new();
                bodyText.Append($"{request.Message}");

                var mailFrom = appSettings.Value.Configurations.MailServer.MailFrom;
                var password = appSettings.Value.Configurations.MailServer.Password;
                var port = appSettings.Value.Configurations.MailServer.Port;
                var smtpServer = appSettings.Value.Configurations.MailServer.SmtpServer;

                var client = new SmtpClient(smtpServer)
                {
                    Port = port,
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new System.Net.NetworkCredential()
                    {
                        UserName = appSettings.Value.Configurations.MailServer.UserMail,
                        Password = password
                    },
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                var message = new MailMessage
                {
                    From = new MailAddress(mailFrom)
                };
                message.IsBodyHtml = true;
                foreach (var mt in request.Receivers.Distinct().ToList())
                {
                    message.To.Add(mt);
                }
                message.Subject = request.Subject;
                message.Body = bodyText.ToString();

                int i = 0;
                foreach (var item in request.Files)
                {
                    Attachment att = new Attachment(new MemoryStream(item), $"{request.FileNames[i]}");
                    message.Attachments.Add(att);
                    i++;
                }

                client.SendMailAsync(message);

                return OperationResult<SendEmailResponse>.CreateSuccessResult(response);
            }
            catch (Exception ex)
            {
                return OperationResult<SendEmailResponse>.CreateFailureResult(ex);
            }
        }
    }
}
