namespace GestionProyectos.Engine.Security.Users;

public class UserAvatarResponse
{
    public bool HasAvatar { get; set; }
    public string FileName { get; set; } = string.Empty;
    public byte[] File { get; set; } = Array.Empty<byte>();
}
