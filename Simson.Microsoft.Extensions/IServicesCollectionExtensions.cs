using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Simson.Microsoft.Extensions
{
    public static class IServicesCollectionExtensions
    {
        public static IServiceCollection ConfigurationOptions<TOptions>(this IServiceCollection services, IConfiguration configuration)
            where TOptions : class
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            var optionsName = typeof(TOptions).Name;
            var configSection = configuration.GetSection(optionsName);
            if (configSection == null)
                throw new KeyNotFoundException($"Options with the token not found: {optionsName}");
            return services.ConfigurationOptions<TOptions>(configSection);
        }
    }
}
