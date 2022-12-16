using ElasticSearch.Repository.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ElasticSearch.Example.Repository
{
    public static class ServiceCollectionExtension
    {
        public static void AddRepository(this IServiceCollection services)
        {
            //注入Repository
            services.AddSingleton<IProductRepository, ProductRepository>();

            //注入Es Repository
            services.AddElasticSearch((seviceProvider, options) =>
            {
                var configuration = seviceProvider.GetService<IConfiguration>();
                configuration.GetSection("ElasticSearchOptions").Bind(options);
            });
        }
    }
}
