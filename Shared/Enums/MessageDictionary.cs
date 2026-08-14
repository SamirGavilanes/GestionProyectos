namespace GestionProyectos.Shared.Enums
{
    public static class MessageDictionary
    {
        public static readonly Messages GenericError = new() { Code = "01", Type = MessageType.Error, Description = "Hubo un problema inesperado." };
        public static readonly Messages WrongPassword = new() { Code = "02", Type = MessageType.Error, Description = "La contraseña ingresada es incorrecta." };
        public static readonly Messages UserNotFound = new() { Code = "03", Type = MessageType.Error, Description = "El usuario no existe." };
    }

    public class Messages
    {
        public string Code { get; set; } = string.Empty;
        public MessageType Type { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public enum MessageType
    {
        Error = 0,
        Success = 1,
        Information = 2
    }
}
