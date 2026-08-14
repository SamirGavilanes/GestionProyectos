namespace GestionProyectos.Engine.Utility.SendEmail.Request
{
    public class SendEmailRequest
    {
        public string Subject { get; set; }
        public List<string> FileNames { get; set; }
        public List<string> Receivers { get; set; }
        public List<byte[]> Files { get; set; }
        public string Message { get; set; }
        public SendEmailRequest()
        {
            Subject = string.Empty;
            FileNames = new List<string>();
            Receivers = new List<string>();
            Files = new List<byte[]>();
            Message = string.Empty;
        }
    }
}
