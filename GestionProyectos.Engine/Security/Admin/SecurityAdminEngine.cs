using GestionProyectos.Data;
using GestionProyectos.Data.Entities.Security;
using GestionProyectos.Engine.Security.Admin.Request;
using GestionProyectos.Engine.Security.Utilities;
using GestionProyectos.Shared.Enums;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Security.Admin
{
    public class SecurityAdminEngine : ISecurityAdminEngine
    {
        private readonly DataDbContext dbContext;

        public SecurityAdminEngine(DataDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        #region USUARIOS
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

        #endregion

        #region ROLES
        public OperationResult<List<Role>> GetRoles()
        {
            try
            {
                var roles = dbContext.Role
                    .Where(r => r.RowStatus == (short)RowStatus.Active)
                    .OrderBy(r => r.Description)
                    .ToList();
                return OperationResult<List<Role>>.CreateSuccessResult(roles);
            }
            catch (Exception ex)
            {
                return OperationResult<List<Role>>.CreateFailureResult(ex);
            }
        }

        public OperationResult<bool> SaveRole(long id, string description, Context context)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(description))
                    return OperationResult<bool>.CreateFailureResult("La descripción es obligatoria.");

                if (id == 0)
                {
                    dbContext.Role.Add(new Role
                    {
                        Description = description.Trim(),
                        RowStatus = (short)RowStatus.Active,
                        Created = DateTime.UtcNow,
                        CreatedBy = context.UserId
                    });
                }
                else
                {
                    var role = dbContext.Role.FirstOrDefault(x => x.Id == id);
                    if (role == null)
                        return OperationResult<bool>.CreateFailureResult("No se encontró el rol.");

                    role.Description = description.Trim();
                    role.Updated = DateTime.UtcNow;
                    role.UpdatedBy = context.UserId;
                }

                dbContext.SaveChanges();
                return OperationResult<bool>.CreateSuccessResult(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.CreateFailureResult(ex);
            }
        }

        public OperationResult<bool> DeleteRole(long id, Context context)
        {
            try
            {
                var role = dbContext.Role.FirstOrDefault(x => x.Id == id);
                if (role == null)
                    return OperationResult<bool>.CreateFailureResult("No se encontró el rol.");

                role.RowStatus = (short)RowStatus.Inactive;
                role.Updated = DateTime.UtcNow;
                role.UpdatedBy = context.UserId;
                dbContext.SaveChanges();
                return OperationResult<bool>.CreateSuccessResult(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.CreateFailureResult(ex);
            }
        }
        #endregion

        #region MENÚS
        public OperationResult<List<Menu>> GetMenus()
        {
            try
            {
                var menus = dbContext.Menu
                    .Where(m => m.RowStatus == (short)RowStatus.Active)
                    .OrderBy(m => m.Order).ThenBy(m => m.Description)
                    .ToList();
                return OperationResult<List<Menu>>.CreateSuccessResult(menus);
            }
            catch (Exception ex)
            {
                return OperationResult<List<Menu>>.CreateFailureResult(ex);
            }
        }

        public OperationResult<bool> SaveMenu(long id, string description, int icon, string page, long? parent, int order, Context context)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(description))
                    return OperationResult<bool>.CreateFailureResult("La descripción es obligatoria.");

                if (id == 0)
                {
                    dbContext.Menu.Add(new Menu
                    {
                        Description = description.Trim(),
                        Icon = icon,
                        Page = page?.Trim() ?? string.Empty,
                        Parent = parent,
                        Order = order,
                        RowStatus = (short)RowStatus.Active,
                        Created = DateTime.UtcNow,
                        CreatedBy = context.UserId
                    });
                }
                else
                {
                    var menu = dbContext.Menu.FirstOrDefault(x => x.Id == id);
                    if (menu == null)
                        return OperationResult<bool>.CreateFailureResult("No se encontró el menú.");

                    menu.Description = description.Trim();
                    menu.Icon = icon;
                    menu.Page = page?.Trim() ?? string.Empty;
                    menu.Parent = parent;
                    menu.Order = order;
                    menu.Updated = DateTime.UtcNow;
                    menu.UpdatedBy = context.UserId;
                }

                dbContext.SaveChanges();
                return OperationResult<bool>.CreateSuccessResult(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.CreateFailureResult(ex);
            }
        }

        public OperationResult<bool> DeleteMenu(long id, Context context)
        {
            try
            {
                var menu = dbContext.Menu.FirstOrDefault(x => x.Id == id);
                if (menu == null)
                    return OperationResult<bool>.CreateFailureResult("No se encontró el menú.");

                menu.RowStatus = (short)RowStatus.Inactive;
                menu.Updated = DateTime.UtcNow;
                menu.UpdatedBy = context.UserId;
                dbContext.SaveChanges();
                return OperationResult<bool>.CreateSuccessResult(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.CreateFailureResult(ex);
            }
        }
        #endregion

        #region USUARIO-ROL
        public OperationResult<List<UserRole>> GetUserRoles()
        {
            try
            {
                var items = dbContext.UserRole
                    .Where(ur => ur.RowStatus == (short)RowStatus.Active)
                    .OrderBy(ur => ur.UserId)
                    .ToList();
                return OperationResult<List<UserRole>>.CreateSuccessResult(items);
            }
            catch (Exception ex)
            {
                return OperationResult<List<UserRole>>.CreateFailureResult(ex);
            }
        }

        public OperationResult<bool> SaveUserRole(long id, long userId, long roleId, Context context)
        {
            try
            {
                if (userId == 0)
                    return OperationResult<bool>.CreateFailureResult("Debe seleccionar un usuario.");
                if (roleId == 0)
                    return OperationResult<bool>.CreateFailureResult("Debe seleccionar un rol.");

                if (id == 0)
                {
                    dbContext.UserRole.Add(new UserRole
                    {
                        UserId = userId,
                        RoleId = roleId,
                        RowStatus = (short)RowStatus.Active,
                        Created = DateTime.UtcNow,
                        CreatedBy = context.UserId
                    });
                }
                else
                {
                    var userRole = dbContext.UserRole.FirstOrDefault(x => x.Id == id);
                    if (userRole == null)
                        return OperationResult<bool>.CreateFailureResult("No se encontró la asignación.");

                    userRole.UserId = userId;
                    userRole.RoleId = roleId;
                    userRole.Updated = DateTime.UtcNow;
                    userRole.UpdatedBy = context.UserId;
                }

                dbContext.SaveChanges();
                return OperationResult<bool>.CreateSuccessResult(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.CreateFailureResult(ex);
            }
        }

        public OperationResult<bool> DeleteUserRole(long id, Context context)
        {
            try
            {
                var userRole = dbContext.UserRole.FirstOrDefault(x => x.Id == id);
                if (userRole == null)
                    return OperationResult<bool>.CreateFailureResult("No se encontró la asignación.");

                userRole.RowStatus = (short)RowStatus.Inactive;
                userRole.Updated = DateTime.UtcNow;
                userRole.UpdatedBy = context.UserId;
                dbContext.SaveChanges();
                return OperationResult<bool>.CreateSuccessResult(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.CreateFailureResult(ex);
            }
        }
        #endregion

        #region ROL-MENÚ
        public OperationResult<List<RoleMenu>> GetRoleMenus()
        {
            try
            {
                var items = dbContext.RoleMenu
                    .Where(rm => rm.RowStatus == (short)RowStatus.Active)
                    .OrderBy(rm => rm.RoleId).ThenBy(rm => rm.MenuId)
                    .ToList();
                return OperationResult<List<RoleMenu>>.CreateSuccessResult(items);
            }
            catch (Exception ex)
            {
                return OperationResult<List<RoleMenu>>.CreateFailureResult(ex);
            }
        }

        public OperationResult<bool> SaveRoleMenu(long id, long roleId, long menuId, Context context)
        {
            try
            {
                if (roleId == 0)
                    return OperationResult<bool>.CreateFailureResult("Debe seleccionar un rol.");
                if (menuId == 0)
                    return OperationResult<bool>.CreateFailureResult("Debe seleccionar un menú.");

                if (id == 0)
                {
                    dbContext.RoleMenu.Add(new RoleMenu
                    {
                        RoleId = roleId,
                        MenuId = menuId,
                        CanView = true,
                        CanCreate = true,
                        CanEdit = true,
                        CanDelete = true,
                        CanRegisterHours = true,
                        RowStatus = (short)RowStatus.Active,
                        Created = DateTime.UtcNow,
                        CreatedBy = context.UserId
                    });
                }
                else
                {
                    var roleMenu = dbContext.RoleMenu.FirstOrDefault(x => x.Id == id);
                    if (roleMenu == null)
                        return OperationResult<bool>.CreateFailureResult("No se encontró la asignación.");

                    roleMenu.RoleId = roleId;
                    roleMenu.MenuId = menuId;
                    roleMenu.Updated = DateTime.UtcNow;
                    roleMenu.UpdatedBy = context.UserId;
                }

                dbContext.SaveChanges();
                return OperationResult<bool>.CreateSuccessResult(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.CreateFailureResult(ex);
            }
        }

        public OperationResult<bool> DeleteRoleMenu(long id, Context context)
        {
            try
            {
                var roleMenu = dbContext.RoleMenu.FirstOrDefault(x => x.Id == id);
                if (roleMenu == null)
                    return OperationResult<bool>.CreateFailureResult("No se encontró la asignación.");

                roleMenu.RowStatus = (short)RowStatus.Inactive;
                roleMenu.Updated = DateTime.UtcNow;
                roleMenu.UpdatedBy = context.UserId;
                dbContext.SaveChanges();
                return OperationResult<bool>.CreateSuccessResult(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.CreateFailureResult(ex);
            }
        }

        public OperationResult<bool> SetRoleMenus(long roleId, List<long> menuIds, Context context)
        {
            try
            {
                if (roleId == 0)
                    return OperationResult<bool>.CreateFailureResult("Debe seleccionar un rol.");

                var active = dbContext.RoleMenu
                    .Where(rm => rm.RoleId == roleId && rm.RowStatus == (short)RowStatus.Active)
                    .ToList();

                var desired = menuIds.Distinct().ToHashSet();

                foreach (var rm in active.Where(rm => !desired.Contains(rm.MenuId)))
                {
                    rm.RowStatus = (short)RowStatus.Inactive;
                    rm.Updated = DateTime.UtcNow;
                    rm.UpdatedBy = context.UserId;
                }

                var existingIds = active.Select(rm => rm.MenuId).ToHashSet();
                foreach (var menuId in desired.Where(id => !existingIds.Contains(id)))
                {
                    dbContext.RoleMenu.Add(new RoleMenu
                    {
                        RoleId = roleId,
                        MenuId = menuId,
                        CanView = true,
                        CanCreate = true,
                        CanEdit = true,
                        CanDelete = true,
                        CanRegisterHours = true,
                        RowStatus = (short)RowStatus.Active,
                        Created = DateTime.UtcNow,
                        CreatedBy = context.UserId
                    });
                }

                dbContext.SaveChanges();
                return OperationResult<bool>.CreateSuccessResult(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.CreateFailureResult(ex);
            }
        }

        public OperationResult<bool> SetRoleMenuPermissions(long roleId, List<RoleMenuPermissionInput> permissions, Context context)
        {
            try
            {
                if (roleId == 0)
                    return OperationResult<bool>.CreateFailureResult("Debe seleccionar un rol.");

                var active = dbContext.RoleMenu
                    .Where(rm => rm.RoleId == roleId && rm.RowStatus == (short)RowStatus.Active)
                    .ToList();

                var desired = permissions
                    .Where(p => p.CanView || p.CanCreate || p.CanEdit || p.CanDelete || p.CanFinalize)
                    .GroupBy(p => p.MenuId)
                    .ToDictionary(g => g.Key, g => g.Last());

                foreach (var rm in active.Where(rm => !desired.ContainsKey(rm.MenuId)))
                {
                    rm.RowStatus = (short)RowStatus.Inactive;
                    rm.Updated = DateTime.UtcNow;
                    rm.UpdatedBy = context.UserId;
                }

                foreach (var entry in desired)
                {
                    var canView = entry.Value.CanView;
                    var canCreate = canView && entry.Value.CanCreate;
                    var canEdit = canView && entry.Value.CanEdit;
                    var canDelete = canView && entry.Value.CanDelete;
                    var canFinalize = canView && entry.Value.CanFinalize;

                    var existing = active.FirstOrDefault(rm => rm.MenuId == entry.Key);
                    if (existing == null)
                    {
                        dbContext.RoleMenu.Add(new RoleMenu
                        {
                            RoleId = roleId,
                            MenuId = entry.Key,
                            CanView = canView,
                            CanCreate = canCreate,
                            CanEdit = canEdit,
                            CanDelete = canDelete,
                            CanRegisterHours = canView,
                            CanFinalize = canFinalize,
                            RowStatus = (short)RowStatus.Active,
                            Created = DateTime.UtcNow,
                            CreatedBy = context.UserId
                        });
                    }
                    else
                    {
                        existing.CanView = canView;
                        existing.CanCreate = canCreate;
                        existing.CanEdit = canEdit;
                        existing.CanDelete = canDelete;
                        existing.CanRegisterHours = canView;
                        existing.CanFinalize = canFinalize;
                        existing.Updated = DateTime.UtcNow;
                        existing.UpdatedBy = context.UserId;
                    }
                }

                var menus = dbContext.Menu
                    .Where(m => m.RowStatus == (short)RowStatus.Active)
                    .ToDictionary(m => m.Id);

                foreach (var entry in desired.Values.Where(p => p.CanView))
                {
                    if (!menus.TryGetValue(entry.MenuId, out var menu) || !menu.Parent.HasValue)
                        continue;

                    var parentId = menu.Parent.Value;
                    if (desired.ContainsKey(parentId))
                        continue;

                    var parentExisting = active.FirstOrDefault(rm => rm.MenuId == parentId);
                    if (parentExisting == null)
                    {
                        dbContext.RoleMenu.Add(new RoleMenu
                        {
                            RoleId = roleId,
                            MenuId = parentId,
                            CanView = true,
                            CanCreate = false,
                            CanEdit = false,
                            CanDelete = false,
                            RowStatus = (short)RowStatus.Active,
                            Created = DateTime.UtcNow,
                            CreatedBy = context.UserId
                        });
                    }
                    else
                    {
                        parentExisting.CanView = true;
                        parentExisting.Updated = DateTime.UtcNow;
                        parentExisting.UpdatedBy = context.UserId;
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
        #endregion
    }
}
