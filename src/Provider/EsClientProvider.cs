using Elasticsearch.Net;
using Microsoft.Extensions.Options;
using Nest;
using System;
using System.Linq;

namespace ElasticSearch.Repository.Provider
{
    /// <summary>
    /// 不用单例模式实现 使用IOC单例注入实现
    /// </summary>
    public class EsClientProvider : IEsClientProvider
    {
        private ElasticSearchOptions options;

        private ElasticClient elasticClient;

        public EsClientProvider(IOptions<ElasticSearchOptions> options)
        {
            if (options == null)
                throw new NullReferenceException("ES未配置");
            this.options = options.Value;
        }

        public ElasticClient GetClient(string indexName)
        {
            if (elasticClient != null)
                return elasticClient;
            var connectionString = options.ConnectionStrings.FirstOrDefault();
            var connectionPool = new SingleNodeConnectionPool(new Uri(connectionString));
            var connectionSetting = new ConnectionSettings(connectionPool);

            if (!string.IsNullOrWhiteSpace(indexName))
                connectionSetting = connectionSetting.DefaultIndex(indexName);

            return new ElasticClient(connectionSetting);
        }
    }
}
