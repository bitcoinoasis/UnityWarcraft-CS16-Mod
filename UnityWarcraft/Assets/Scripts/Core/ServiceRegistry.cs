using System;
using System.Collections.Generic;

namespace Warcraft.Core
{
    /// <summary>
    /// Lightweight service locator for non-MonoBehaviour services. Keeps registration explicit and discoverable.
    /// </summary>
    public static class ServiceRegistry
    {
        private static readonly Dictionary<Type, object> Services = new();

        public static void Register<TService>(TService service, bool overwrite = false)
            where TService : class
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            var key = typeof(TService);

            if (Services.ContainsKey(key) && !overwrite)
            {
                throw new InvalidOperationException($"Service of type {key.Name} already registered.");
            }

            Services[key] = service;
        }

        public static bool TryGet<TService>(out TService service) where TService : class
        {
            if (Services.TryGetValue(typeof(TService), out var boxed) && boxed is TService typed)
            {
                service = typed;
                return true;
            }

            service = null;
            return false;
        }

        public static TService Get<TService>() where TService : class
        {
            if (TryGet<TService>(out var service))
            {
                return service;
            }

            throw new InvalidOperationException($"Service of type {typeof(TService).Name} is not registered.");
        }

        public static void Clear()
        {
            Services.Clear();
        }
    }
}
