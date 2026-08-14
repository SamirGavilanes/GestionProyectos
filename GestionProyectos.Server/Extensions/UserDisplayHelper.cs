namespace GestionProyectos.Server.Extensions;

public static class UserDisplayHelper
{
    public static string GetInitials(string? fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return "?";

        var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length >= 2
            ? $"{char.ToUpperInvariant(parts[0][0])}{char.ToUpperInvariant(parts[^1][0])}"
            : parts[0][..Math.Min(2, parts[0].Length)].ToUpperInvariant();
    }

    public static string GetContentType(string fileName) =>
        Path.GetExtension(fileName).ToLowerInvariant() switch
        {
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".bmp" => "image/bmp",
            _ => "application/octet-stream"
        };

    public static string ToDataUrl(byte[] fileBytes, string fileName)
    {
        var contentType = GetContentType(fileName);
        return $"data:{contentType};base64,{Convert.ToBase64String(fileBytes)}";
    }
}
