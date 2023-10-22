using System;
using System.Collections.Generic;
using System.Text;

namespace FLZ.Services
{
    public static class ServiceProvider
    {
        private static readonly Dictionary<Type, List<object>> _servicesDictionary = new Dictionary<Type, List<object>>();

        /// <summary>
        /// Get the first service of the type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetService<T>() where T : class
        {
            Type interfaceType = typeof(T);

            _servicesDictionary.TryGetValue(interfaceType, out var service);
            return service?.Count > 0 ? (T) service[0] : null;
        }

        /// <summary>
        /// Get the all services of the type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> GetServices<T>() where T : class
        {
            _servicesDictionary.TryGetValue(typeof(T), out var services);

            if (services != null)
            {
                List<T> convertedServices = new List<T>(services.Count);

                for (int i = 0; i < services.Count; i++)
                {
                    convertedServices.Add((T)services[i]);
                }

                return convertedServices;
            }

            return null;
        }
        
        public static void AddService(Type @interface, object service)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            _servicesDictionary.TryGetValue(@interface, out var services);
            if (services == null)
            {
                services = new List<object>();
                _servicesDictionary.Add(@interface, services);
            }
            else
            {
                var dup = services.Find(x => x.GetType() == service.GetType());
                if (dup != null)
                    throw new Exception("Adding already existing Service " + service.GetType());
            }
            
            services.Add(service);
        }

        public static void AddService<T>(T service)
        {
            AddService(typeof(T), service);
        }
        
        public static void RemoveService<T>(T service)
        {
            _servicesDictionary.TryGetValue(service.GetType(), out var services);
            services?.Remove(service);

            if (services?.Count == 0)
                _servicesDictionary.Remove(service.GetType());
        }
        
        public static void RemoveService<T>() where T : IService
        {
            _servicesDictionary.Remove(typeof(T));
        }

        public static StringBuilder Report(StringBuilder builder)
        {
            builder.AppendLine("Current services");

            foreach (var kvp in _servicesDictionary)
            {
                builder.Append($"{kvp.Key.Name}:");
                foreach (var obj in kvp.Value)
                {
                    builder.Append($" {obj}");
                }
                builder.AppendLine();
            }

            return builder;
        }

        
        public static void Dispose()
        {
            _servicesDictionary.Clear();
        }
    }
}