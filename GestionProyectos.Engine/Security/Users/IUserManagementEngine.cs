using GestionProyectos.Data.Entities.Security;
using GestionProyectos.Engine.Security.Utilities;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Security.Users
{
    public interface IUserManagementEngine
    {
        OperationResult<List<User>> GetUsers();
        OperationResult<bool> SaveUser(long id, string name, string lastName, string email, string password, string jobTitle, long roleId, Context context);
        OperationResult<bool> DeleteUser(long id, Context context);

        OperationResult<List<UserTimeOff>> GetTimeOffs();
        OperationResult<bool> SaveTimeOff(long id, long userId, short type, DateTime startDate, DateTime endDate, decimal hours, string description, Context context);
        OperationResult<bool> DeleteTimeOff(long id, Context context);
        OperationResult<bool> ChangePassword(long userId, string currentPassword, string newPassword, Context context);
        OperationResult<bool> UploadAvatar(long userId, string fileName, Stream fileStream, Context context);
        OperationResult<UserAvatarResponse> GetAvatar(long userId, Context context);
        OperationResult<bool> RemoveAvatar(long userId, Context context);
    }
}
