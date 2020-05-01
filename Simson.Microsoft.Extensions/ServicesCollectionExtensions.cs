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

        public static void AddEachImplementation<TService>(this IServiceCollection services, ServiceLifetime lifetime)
        {
            var curAssembly = Assembly.GetEntryAssembly();
            AddEachImplementation<TService>(services, lifetime, curAssembly);
        }

        public static void AddEachImplementation<TService>(this IServiceCollection services, ServiceLifetime lifetime, params Assembly[] assemblies)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            var types = GetAssignableTypes<TService>(assemblies);
            foreach (var type in types)
                services.Add(new ServiceDescriptor(type, lifetime));
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
            var descriptor = new ServiceDescriptor(typeof(List<TService>), provider => provider.GetServices<TService>(), lifetime);
            services.Add(descriptor);
        }

        private static List<Type> GetAssignableTypes<T>(params Assembly[] assemblies)
        {
            if (assemblies == null)
                throw new ArgumentNullException(nameof(assemblies));
            var result = new List<Type>();
            var type = typeof(T);
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes()
                    .Where(x => type.IsAssignableFrom(x));
                result.AddRange(types);
            }
            return result;
        }
    }
}
