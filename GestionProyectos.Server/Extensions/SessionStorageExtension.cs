using Blazored.SessionStorage;
using Newtonsoft.Json;

namespace GestionProyectos.Server.Extensions
{
    public static class SessionStorageExtension
    {
        public static async Task SaveStorage<T>(this ISessionStorageService sessionStorageService, string key, T item) where T : class
        {
            var itemJson = JsonConvert.SerializeObject(item);

            await sessionStorageService.SetItemAsStringAsync(key, itemJson);
        }

        public static async Task<T?> GetStorage<T>(this ISessionStorageService sessionStorageService, string key) where T : class
        {
            var itemJson = await sessionStorageService.GetItemAsStringAsync(key);

            if (itemJson != null)
            {
                return JsonConvert.DeserializeObject<T>(itemJson);
            }

            return null;
        }
    }
}
