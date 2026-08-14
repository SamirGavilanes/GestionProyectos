
using GestionProyectos.Shared.Models.UploadFile;

namespace GestionProyectos.Shared.Configurations
{
    public class ConfigurationsManager
    {
        public string NameListener { get; set; } = string.Empty;
        public string IPListener { get; set; } = string.Empty;
        public string PortListener { get; set; } = string.Empty;
        public string IpPortListener { get { return $"{IPListener}:{PortListener}"; } }
        public string NetworkAdapter { get; set; } = string.Empty;
        public bool Encription { get; set; }
        public int ReceiveBufferSize { get; set; }
        public string AwsSecretManager { get; set; } = string.Empty;
        public string AwsVersionStage { get; set; } = string.Empty;
        public string AwsRegion { get; set; } = string.Empty;
        public bool UseInMemoryDatabase { get; set; }
        public ConnectionStringsManager ConnectionStrings { get; set; } = null!;
        public S3Config S3Config { get; set; } = null!;
        public MailServer MailServer { get; set; } = null!;
        public Logging Logging { get; set; } = null!;
    }

    public class ConnectionStringsManager
    {
        public string GestionProyectos { get; set; } = string.Empty;
    }

    public class Logging
    {
        public string LoggerHubURL { get; set; } = string.Empty;
        public string LoggerFilePath { get; set; } = string.Empty;
        public string LoggerApplicationId { get; set; } = string.Empty;
    }

    public class MailServer
    {
        public string MailFrom { get; set; } = string.Empty;
        public string UserMail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string SmtpServer { get; set; } = string.Empty;
        public int Port { get; set; }
        public string TemplatePath { get; set; } = string.Empty;
        public List<string> NotificationEmails { get; set; } = new();
        public string DownloadUrl { get; set; } = string.Empty;
    }

}
