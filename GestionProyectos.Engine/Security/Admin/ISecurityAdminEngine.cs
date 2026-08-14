using GestionProyectos.Data.Entities.Security;
using GestionProyectos.Engine.Security.Admin.Request;
using GestionProyectos.Engine.Security.Utilities;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Security.Admin
{
    public interface ISecurityAdminEngine
    {
        OperationResult<List<User>> GetUsers();

        OperationResult<List<Role>> GetRoles();
        OperationResult<bool> SaveRole(long id, string description, Context context);
        OperationResult<bool> DeleteRole(long id, Context context);

        OperationResult<List<Menu>> GetMenus();
        OperationResult<bool> SaveMenu(long id, string description, int icon, string page, long? parent, int order, Context context);
        OperationResult<bool> DeleteMenu(long id, Context context);

        OperationResult<List<UserRole>> GetUserRoles();
        OperationResult<bool> SaveUserRole(long id, long userId, long roleId, Context context);
        OperationResult<bool> DeleteUserRole(long id, Context context);

        OperationResult<List<RoleMenu>> GetRoleMenus();
        OperationResult<bool> SaveRoleMenu(long id, long roleId, long menuId, Context context);
        OperationResult<bool> DeleteRoleMenu(long id, Context context);
        OperationResult<bool> SetRoleMenus(long roleId, List<long> menuIds, Context context);
        OperationResult<bool> SetRoleMenuPermissions(long roleId, List<RoleMenuPermissionInput> permissions, Context context);
    }
}
