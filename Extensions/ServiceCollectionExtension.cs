using Elasticsearch.Net;
using ElasticSearch.Repository.Provider;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nest;
using System;
using System.Linq;

namespace ElasticSearch.Repository.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddElasticSearch(this IServiceCollection services, string defaultIndexName)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetService<IConfiguration>();

            //services.AddSingleton<IEsClientProvider, EsClientProvider>();
            services.Configure<ElasticSearchOptions>(configuration.GetSection("ElasticSearchOptions"));

            services.AddSingleton<IElasticClient>(provider =>
            {
                var options = provider.GetService<IOptions<ElasticSearchOptions>>();
                if (options == null || options.Value == null)
                    throw new ArgumentNullException("ElasticSearch未配置");
                var connectionString = options.Value.ConnectionStrings.FirstOrDefault();
                var connectionPool = new SingleNodeConnectionPool(new Uri(connectionString));
                var connectionSetting = new ConnectionSettings(connectionPool).DisableDirectStreaming();

                if (!string.IsNullOrWhiteSpace(defaultIndexName))
                    connectionSetting = connectionSetting.DefaultIndex(defaultIndexName);
                return new ElasticClient(connectionSetting);
            });


            return services;
        }
    }
}
