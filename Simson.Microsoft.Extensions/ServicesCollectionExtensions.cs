using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Simson.Microsoft.Extensions
{
    public static class ServicesCollectionExtensions
    {
        public static IServiceCollection ConfigureOptions<TOptions>(this IServiceCollection services, IConfiguration configuration)
            where TOptions : class
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            var optionsName = typeof(TOptions).Name;
            var configSection = configuration.GetSection(optionsName);
            if (configSection == null)
                throw new KeyNotFoundException($"Configuration section with the token not found: {optionsName}");
            return services.Configure<TOptions>(configSection);
        }

        public static void AddImplementationsToSelf<TService>(this IServiceCollection services, ServiceLifetime lifetime)
        {
            var curAssembly = Assembly.GetEntryAssembly();
            AddImplementationsToSelf<TService>(services, lifetime, curAssembly);
        }

        public static void AddImplementationsToSelf<TService>(this IServiceCollection services, ServiceLifetime lifetime, params Assembly[] assemblies)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            var types = GetAssignableTypes<TService>(assemblies);
            services.AddServices(types, lifetime);
        }

        public static void AddImplementations<TService>(this IServiceCollection services, ServiceLifetime lifetime)
        {
            var curAssembly = Assembly.GetEntryAssembly();
            AddImplementations<TService>(services, lifetime, curAssembly);
        }

        public static void AddImplementations<TService>(this IServiceCollection services, ServiceLifetime lifetime, params Assembly[] assemblies)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            var types = GetAssignableTypes<TService>(assemblies);
            services.AddServices<TService>(types, lifetime);
        }

        private static void AddServices(this IServiceCollection services, IEnumerable<Type> types, ServiceLifetime lifetime)
        {
            foreach (var type in types)
                services.Add(new ServiceDescriptor(type, type, lifetime));
        }
        
        private static void AddServices<TService>(this IServiceCollection services, IEnumerable<Type> types, ServiceLifetime lifetime)
        {
            var serviceType = typeof(TService);
            foreach (var type in types)
                services.Add(new ServiceDescriptor(serviceType, type, lifetime));
        }

        private static IEnumerable<Type> GetAssignableTypes<T>(params Assembly[] assemblies)
        {
            if (assemblies == null)
                throw new ArgumentNullException(nameof(assemblies));
            var result = new List<Type>();
            var type = typeof(T);
            foreach (var assembly in assemblies)
            {
                if (assembly == null)
                    throw new ArgumentNullException($"Entries of {nameof(assemblies)} cannot be null");
                var types = assembly.GetTypes()
                    .Where(x => x.IsClass && !x.IsAbstract && type.IsAssignableFrom(x));
                result.AddRange(types);
            }
            return result;
        }
    }
}
