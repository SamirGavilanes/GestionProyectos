using Blazored.SessionStorage;
using GestionProyectos.Engine.Security;
using GestionProyectos.Engine.Security.Utilities;

namespace GestionProyectos.Server.Extensions;

public static class SessionMenuExtensions
{
    public static async Task<Context?> RefreshMenusAsync(
        this ISessionStorageService sessionStorage,
        ISecurityEngine securityEngine)
    {
        var context = await sessionStorage.GetStorage<Context>("session");
        if (context == null || context.UserId <= 0)
            return context;

        var menus = securityEngine.GetMenusForUser(context.UserId);
        if (menus.Count == 0)
            return context;

        context.Menus = menus;
        await sessionStorage.SaveStorage("session", context);
        return context;
    }
}
