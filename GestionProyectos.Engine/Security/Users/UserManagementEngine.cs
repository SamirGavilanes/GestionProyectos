using GestionProyectos.Data;
using GestionProyectos.Data.Entities.Security;
using GestionProyectos.Engine.Security.Utilities;
using GestionProyectos.Engine.Utility.S3DownloadFile;
using GestionProyectos.Engine.Utility.S3DownloadFile.Request;
using GestionProyectos.Engine.Utility.S3UploadFile;
using GestionProyectos.Engine.Utility.S3UploadFile.Request;
using GestionProyectos.Shared.Configurations;
using GestionProyectos.Shared.Enums;
using GestionProyectos.Shared.Message;
using Microsoft.Extensions.Options;

namespace GestionProyectos.Engine.Security.Users
{
    public class UserManagementEngine : IUserManagementEngine
    {
        private const int MaxAvatarBytes = 512_000;

        private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".png", ".jpg", ".jpeg", ".gif", ".webp", ".bmp"
        };

        private readonly DataDbContext dbContext;
        private readonly IS3UploadFileEngine s3UploadFileEngine;
        private readonly IS3DownloadFileEngine s3DownloadFileEngine;
        private readonly IOptions<AppSettingsManagerBase> appSettings;

        public UserManagementEngine(
            DataDbContext dbContext,
            IS3UploadFileEngine s3UploadFileEngine,
            IS3DownloadFileEngine s3DownloadFileEngine,
            IOptions<AppSettingsManagerBase> appSettings)
        {
            this.dbContext = dbContext;
            this.s3UploadFileEngine = s3UploadFileEngine;
            this.s3DownloadFileEngine = s3DownloadFileEngine;
            this.appSettings = appSettings;
        }

        public OperationResult<List<User>> GetUsers()
        {
            try
            {
                var users = dbContext.User
                    .Where(u => u.RowStatus == (short)RowStatus.Active)
                    .OrderBy(u => u.LastName).ThenBy(u => u.Name)
                    .ToList();
                return OperationResult<List<User>>.CreateSuccessResult(users);
            }
            catch (Exception ex)
            {
                return OperationResult<List<User>>.CreateFailureResult(ex);
            }
        }

        public OperationResult<bool> SaveUser(long id, string name, string lastName, string email, string password, string jobTitle, long roleId, Context context)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    return OperationResult<bool>.CreateFailureResult("El nombre es obligatorio.");
                if (string.IsNullOrWhiteSpace(lastName))
                    return OperationResult<bool>.CreateFailureResult("El apellido es obligatorio.");
                if (string.IsNullOrWhiteSpace(email))
                    return OperationResult<bool>.CreateFailureResult("El correo es obligatorio.");
                if (roleId == 0)
                    return OperationResult<bool>.CreateFailureResult("Debe seleccionar un rol.");

                if (id == 0)
                {
                    if (string.IsNullOrWhiteSpace(password))
                        return OperationResult<bool>.CreateFailureResult("La contraseña es obligatoria.");

                    var user = new User
                    {
                        Name = name.Trim(),
                        LastName = lastName.Trim(),
                        Email = email.Trim(),
                        Password = password,
                        JobTitle = jobTitle?.Trim() ?? string.Empty,
                        RowStatus = (short)RowStatus.Active,
                        Created = DateTime.UtcNow,
                        CreatedBy = context.UserId
                    };
                    dbContext.User.Add(user);
                    dbContext.SaveChanges();

                    dbContext.UserRole.Add(new UserRole
                    {
                        UserId = user.Id,
                        RoleId = roleId,
                        RowStatus = (short)RowStatus.Active,
                        Created = DateTime.UtcNow,
                        CreatedBy = context.UserId
                    });
                }
                else
                {
                    var user = dbContext.User.FirstOrDefault(x => x.Id == id);
                    if (user == null)
                        return OperationResult<bool>.CreateFailureResult("No se encontró el usuario.");

                    user.Name = name.Trim();
                    user.LastName = lastName.Trim();
                    user.Email = email.Trim();
                    user.JobTitle = jobTitle?.Trim() ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(password))
                        user.Password = password;
                    user.Updated = DateTime.UtcNow;
                    user.UpdatedBy = context.UserId;

                    var userRole = dbContext.UserRole
                        .FirstOrDefault(ur => ur.UserId == id && ur.RowStatus == (short)RowStatus.Active);
                    if (userRole == null)
                    {
                        dbContext.UserRole.Add(new UserRole
                        {
                            UserId = id,
                            RoleId = roleId,
                            RowStatus = (short)RowStatus.Active,
                            Created = DateTime.UtcNow,
                            CreatedBy = context.UserId
                        });
                    }
                    else
                    {
                        userRole.RoleId = roleId;
                        userRole.Updated = DateTime.UtcNow;
                        userRole.UpdatedBy = context.UserId;
                    }
                }

                dbContext.SaveChanges();
                return OperationResult<bool>.CreateSuccessResult(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.CreateFailureResult(ex);
            }
        }

        public OperationResult<bool> DeleteUser(long id, Context context)
        {
            try
            {
                var user = dbContext.User.FirstOrDefault(x => x.Id == id);
                if (user == null)
                    return OperationResult<bool>.CreateFailureResult("No se encontró el usuario.");

                user.RowStatus = (short)RowStatus.Inactive;
                user.Updated = DateTime.UtcNow;
                user.UpdatedBy = context.UserId;
                dbContext.SaveChanges();
                return OperationResult<bool>.CreateSuccessResult(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.CreateFailureResult(ex);
            }
        }

        public OperationResult<List<UserTimeOff>> GetTimeOffs()
        {
            try
            {
                var items = dbContext.UserTimeOff
                    .Where(x => x.RowStatus == (short)RowStatus.Active)
                    .OrderByDescending(x => x.StartDate)
                    .ThenBy(x => x.UserId)
                    .ToList();
                return OperationResult<List<UserTimeOff>>.CreateSuccessResult(items);
            }
            catch (Exception ex)
            {
                return OperationResult<List<UserTimeOff>>.CreateFailureResult(ex);
            }
        }

        public OperationResult<bool> SaveTimeOff(long id, long userId, short type, DateTime startDate, DateTime endDate, decimal hours, string description, Context context)
        {
            try
            {
                if (userId == 0)
                    return OperationResult<bool>.CreateFailureResult("Debe seleccionar un usuario.");
                if (type != (short)UserTimeOffType.Vacation && type != (short)UserTimeOffType.Permission)
                    return OperationResult<bool>.CreateFailureResult("El tipo de ausencia no es válido.");
                if (endDate.Date < startDate.Date)
                    return OperationResult<bool>.CreateFailureResult("La fecha fin no puede ser anterior a la fecha inicio.");
                if (hours < 0)
                    return OperationResult<bool>.CreateFailureResult("Las horas no pueden ser negativas.");

                if (id == 0)
                {
                    dbContext.UserTimeOff.Add(new UserTimeOff
                    {
                        UserId = userId,
                        Type = type,
                        StartDate = startDate.Date,
                        EndDate = endDate.Date,
                        Hours = hours,
                        Description = description?.Trim() ?? string.Empty,
                        RowStatus = (short)RowStatus.Active,
                        Created = DateTime.UtcNow,
                        CreatedBy = context.UserId
                    });
                }
                else
                {
                    var item = dbContext.UserTimeOff.FirstOrDefault(x => x.Id == id);
                    if (item == null)
                        return OperationResult<bool>.CreateFailureResult("No se encontró el registro.");

                    item.UserId = userId;
                    item.Type = type;
                    item.StartDate = startDate.Date;
                    item.EndDate = endDate.Date;
                    item.Hours = hours;
                    item.Description = description?.Trim() ?? string.Empty;
                    item.Updated = DateTime.UtcNow;
                    item.UpdatedBy = context.UserId;
                }

                dbContext.SaveChanges();
                return OperationResult<bool>.CreateSuccessResult(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.CreateFailureResult(ex);
            }
        }

        public OperationResult<bool> DeleteTimeOff(long id, Context context)
        {
            try
            {
                var item = dbContext.UserTimeOff.FirstOrDefault(x => x.Id == id);
                if (item == null)
                    return OperationResult<bool>.CreateFailureResult("No se encontró el registro.");

                item.RowStatus = (short)RowStatus.Inactive;
                item.Updated = DateTime.UtcNow;
                item.UpdatedBy = context.UserId;
                dbContext.SaveChanges();
                return OperationResult<bool>.CreateSuccessResult(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.CreateFailureResult(ex);
            }
        }

        public OperationResult<bool> ChangePassword(long userId, string currentPassword, string newPassword, Context context)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(currentPassword))
                    return OperationResult<bool>.CreateFailureResult("La contraseña actual es obligatoria.");
                if (string.IsNullOrWhiteSpace(newPassword))
                    return OperationResult<bool>.CreateFailureResult("La nueva contraseña es obligatoria.");
                if (newPassword.Length < 6)
                    return OperationResult<bool>.CreateFailureResult("La nueva contraseña debe tener al menos 6 caracteres.");

                var user = dbContext.User.FirstOrDefault(u => u.Id == userId && u.RowStatus == (short)RowStatus.Active);
                if (user == null)
                    return OperationResult<bool>.CreateFailureResult("No se encontró el usuario.");

                if (user.Password != currentPassword)
                    return OperationResult<bool>.CreateFailureResult("La contraseña actual no es correcta.");

                user.Password = newPassword;
                user.Updated = DateTime.UtcNow;
                user.UpdatedBy = context.UserId;
                dbContext.SaveChanges();
                return OperationResult<bool>.CreateSuccessResult(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.CreateFailureResult(ex);
            }
        }

        public OperationResult<bool> UploadAvatar(long userId, string fileName, Stream fileStream, Context context)
        {
            try
            {
                var accessError = ValidateAvatarAccess(userId, context);
                if (accessError != null)
                    return OperationResult<bool>.CreateFailureResult(accessError);

                if (string.IsNullOrWhiteSpace(fileName))
                    return OperationResult<bool>.CreateFailureResult("Seleccione una imagen.");

                var extension = Path.GetExtension(fileName);
                if (!AllowedImageExtensions.Contains(extension))
                    return OperationResult<bool>.CreateFailureResult("Formato no permitido. Use PNG, JPG, GIF, WEBP o BMP.");

                if (fileStream.Length > MaxAvatarBytes)
                    return OperationResult<bool>.CreateFailureResult("La imagen supera 512 KB.");

                var user = dbContext.User.FirstOrDefault(u => u.Id == userId && u.RowStatus == (short)RowStatus.Active);
                if (user == null)
                    return OperationResult<bool>.CreateFailureResult("No se encontró el usuario.");

                var storedFileName = $"avatar{extension.ToLowerInvariant()}";
                var destinationPath = BuildAvatarPath(userId);
                var uploadRequest = new S3UploadFileRequest
                {
                    AccessKey = appSettings.Value.Configurations.S3Config.AccessKey,
                    BuketName = appSettings.Value.Configurations.S3Config.BuketName,
                    SecretAccessKey = appSettings.Value.Configurations.S3Config.SecretAccessKey,
                    DestinationPath = destinationPath,
                    Active = appSettings.Value.Configurations.S3Config.Active,
                    Name = storedFileName,
                    File = fileStream
                };

                var uploadResponse = s3UploadFileEngine.Execute(uploadRequest);
                if (!uploadResponse.Success)
                    return OperationResult<bool>.CreateFailureResult(uploadResponse.Message.Description);

                user.AvatarFileName = storedFileName;
                user.AvatarFilePath = destinationPath;
                user.Updated = DateTime.UtcNow;
                user.UpdatedBy = context.UserId;
                dbContext.SaveChanges();

                return OperationResult<bool>.CreateSuccessResult(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.CreateFailureResult(ex);
            }
        }

        public OperationResult<UserAvatarResponse> GetAvatar(long userId, Context context)
        {
            try
            {
                var accessError = ValidateAvatarAccess(userId, context);
                if (accessError != null)
                    return OperationResult<UserAvatarResponse>.CreateFailureResult(accessError);

                var user = dbContext.User.FirstOrDefault(u => u.Id == userId && u.RowStatus == (short)RowStatus.Active);
                if (user == null)
                    return OperationResult<UserAvatarResponse>.CreateFailureResult("No se encontró el usuario.");

                if (string.IsNullOrWhiteSpace(user.AvatarFileName) || string.IsNullOrWhiteSpace(user.AvatarFilePath))
                {
                    return OperationResult<UserAvatarResponse>.CreateSuccessResult(new UserAvatarResponse
                    {
                        HasAvatar = false
                    });
                }

                var downloadRequest = new S3DownloadFileRequest
                {
                    AccessKey = appSettings.Value.Configurations.S3Config.AccessKey,
                    BuketName = appSettings.Value.Configurations.S3Config.BuketName,
                    SecretAccessKey = appSettings.Value.Configurations.S3Config.SecretAccessKey,
                    Active = appSettings.Value.Configurations.S3Config.Active,
                    FilePath = $"{user.AvatarFilePath}{user.AvatarFileName}"
                };

                var fileResponse = s3DownloadFileEngine.Execute(downloadRequest);
                if (!fileResponse.Success || fileResponse.Data?.File == null || fileResponse.Data.File.Length == 0)
                {
                    return OperationResult<UserAvatarResponse>.CreateSuccessResult(new UserAvatarResponse
                    {
                        HasAvatar = false
                    });
                }

                return OperationResult<UserAvatarResponse>.CreateSuccessResult(new UserAvatarResponse
                {
                    HasAvatar = true,
                    FileName = user.AvatarFileName,
                    File = fileResponse.Data.File
                });
            }
            catch (Exception ex)
            {
                return OperationResult<UserAvatarResponse>.CreateFailureResult(ex);
            }
        }

        public OperationResult<bool> RemoveAvatar(long userId, Context context)
        {
            try
            {
                var accessError = ValidateAvatarAccess(userId, context);
                if (accessError != null)
                    return OperationResult<bool>.CreateFailureResult(accessError);

                var user = dbContext.User.FirstOrDefault(u => u.Id == userId && u.RowStatus == (short)RowStatus.Active);
                if (user == null)
                    return OperationResult<bool>.CreateFailureResult("No se encontró el usuario.");

                user.AvatarFileName = null;
                user.AvatarFilePath = null;
                user.Updated = DateTime.UtcNow;
                user.UpdatedBy = context.UserId;
                dbContext.SaveChanges();

                return OperationResult<bool>.CreateSuccessResult(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.CreateFailureResult(ex);
            }
        }

        private static string BuildAvatarPath(long userId) =>
            $"GestionProyectosQA/users/{userId}/";

        private string? ValidateAvatarAccess(long userId, Context context)
        {
            if (context.UserId <= 0)
                return "Sesión no válida.";

            if (context.UserId == userId)
                return null;

            if (context.Role?.Equals("Administrador", StringComparison.OrdinalIgnoreCase) == true)
                return null;

            return "No tiene permiso para modificar este avatar.";
        }
    }
}
