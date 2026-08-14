using GestionProyectos.Data;
using GestionProyectos.Engine.Security.Request;
using GestionProyectos.Engine.Security.Response;
using GestionProyectos.Engine.Security.Utilities;
using GestionProyectos.Shared.Enums;
using GestionProyectos.Shared.Message;
using Microsoft.EntityFrameworkCore;

namespace GestionProyectos.Engine.Security
{
    public class SecurityEngine : ISecurityEngine
    {
        private readonly DataDbContext dbContext;
        public SecurityEngine(DataDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public OperationResult<LoginResponse> Login(LoginRequest request)
        {
            try
            {
                Context sessionService = new();
                LoginResponse loginResponse = new();

                var userRole = dbContext.UserRole
                    .Include(u => u.User)
                    .Include(u => u.Role)
                        .ThenInclude(r => r.RoleMenus)
                        .ThenInclude(rm => rm.Menu)
                    .FirstOrDefault(u => u.User.Email == request.UserName && u.RowStatus == (short)RowStatus.Active);

                if (userRole == null)
                    return OperationResult<LoginResponse>.CreateFailureResult(new Message() { Code = MessageDictionary.UserNotFound.Code, Description = MessageDictionary.UserNotFound.Description });

                if (request.Password == userRole.User.Password)
                {
                    sessionService.Email = userRole.User.Email;
                    sessionService.Name = $"{userRole.User.Name} {userRole.User.LastName}";
                    sessionService.UserId = userRole.User.Id;
                    sessionService.Role = userRole.Role.Description;
                    sessionService.Menus = GetMenusForUser(userRole.User.Id);

                    loginResponse.Session = sessionService;
                    return OperationResult<LoginResponse>.CreateSuccessResult(loginResponse);
                }

                return OperationResult<LoginResponse>.CreateFailureResult(new Message() { Code = MessageDictionary.WrongPassword.Code, Description = MessageDictionary.WrongPassword.Description });
            }
            catch (Exception)
            {
                return OperationResult<LoginResponse>.CreateFailureResult("No se pudo iniciar sesión.");
            }
        }

        public PagePermissions GetPermissionsForUser(long userId, string route)
        {
            try
            {
                var menus = GetMenusForUser(userId);
                return PermissionHelper.Resolve(new Context { Menus = menus }, route);
            }
            catch
            {
                return PagePermissions.None;
            }
        }

        public List<MenuItem> GetMenusForUser(long userId)
        {
            try
            {
                var userRole = dbContext.UserRole
                    .Include(u => u.User)
                    .Include(u => u.Role)
                        .ThenInclude(r => r.RoleMenus)
                        .ThenInclude(rm => rm.Menu)
                    .FirstOrDefault(u => u.UserId == userId && u.RowStatus == (short)RowStatus.Active);

                if (userRole?.Role?.RoleMenus == null)
                    return new List<MenuItem>();

                return userRole.Role.RoleMenus
                    .Where(rm => rm.RowStatus == (short)RowStatus.Active
                                 && rm.Menu != null
                                 && rm.Menu.RowStatus == (short)RowStatus.Active)
                    .Select(rm => new MenuItem
                    {
                        Id = rm.MenuId,
                        Description = rm.Menu.Description,
                        Icon = rm.Menu.Icon,
                        Page = rm.Menu.Page,
                        Parent = rm.Menu.Parent,
                        Oder = rm.Menu.Order,
                        CanView = rm.CanView,
                        CanCreate = rm.CanCreate,
                        CanEdit = rm.CanEdit,
                        CanDelete = rm.CanDelete,
                        CanRegisterHours = rm.CanRegisterHours,
                        CanFinalize = rm.CanFinalize
                    })
                    .OrderBy(m => m.Oder)
                    .ToList();
            }
            catch
            {
                return new List<MenuItem>();
            }
        }
    }
}
