using Elasticsearch.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nest;
using System;
using System.Linq;

namespace ElasticSearch.Repository.Extensions
{
    public static class ServiceCollectionExtension
    {

        public static IServiceCollection AddElasticSearch(this IServiceCollection services, Action<ElasticSearchOptions> configAction)
             => AddElasticSearch(services, (serviceProvider, options) => configAction.Invoke(options));


        public static IServiceCollection AddElasticSearch(this IServiceCollection services, Action<IServiceProvider, ElasticSearchOptions> configAction)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (configAction == null)
                throw new ArgumentNullException(nameof(configAction));

            services.AddOptions<ElasticSearchOptions>()
                    .Configure<IServiceProvider>((options, sp) => configAction(sp, options));

            services.AddSingleton<IElasticClient>(provider =>
            {
                var options = provider.GetService<IOptions<ElasticSearchOptions>>();
                var connectionString = options.Value.ConnectionStrings.FirstOrDefault();
                var connectionPool = new SingleNodeConnectionPool(new Uri(connectionString));
                var connectionSetting = new ConnectionSettings(connectionPool);
                if (options.Value.DisableDirectStreaming)
                    connectionSetting.DisableDirectStreaming();
                return new ElasticClient(connectionSetting);
            });

            return services;
        }
    }
}
