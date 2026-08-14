using Blazored.SessionStorage;
using GestionProyectos.Engine.Security.Utilities;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace GestionProyectos.Server.Extensions
{
    public class AuthenticationExtension : AuthenticationStateProvider
    {
        private readonly ISessionStorageService sessionStorage;

        public AuthenticationExtension(ISessionStorageService _sessionStorage)
        {
            sessionStorage = _sessionStorage;
        }

        public async Task UpdateAuthenticationState(Context? session)
        {
            ClaimsPrincipal claimsPrincipal;

            if (session != null)
            {
                claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim("Email", session.Email),
                    new Claim("Name", session.Name),
                    new Claim("Role", session.Role),
                    new Claim("UserId", session.UserId.ToString())
                }, "JwtAuth"));

                await sessionStorage.SaveStorage("session", session);
            }
            else
            {
                claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
                await sessionStorage.RemoveItemAsync("session");
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var session = await sessionStorage.GetStorage<Context>("session");

            if (session == null)
                return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim("Email", session.Email),
                    new Claim("Name", session.Name),
                    new Claim("Role", session.Role),
                    new Claim("UserId", session.UserId.ToString())
                }, "JwtAuth"));

            return await Task.FromResult(new AuthenticationState(claimsPrincipal));
        }
    }
}
