using ElasticSearch.Repository.Provider;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ElasticSearch.Repository.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddElasticSearch(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetService<IConfiguration>();

            services.AddSingleton<IEsClientProvider, EsClientProvider>();
            services.Configure<ElasticSearchOptions>(configuration.GetSection("ElasticSearchOptions"));
            return services;
        }
    }
}
