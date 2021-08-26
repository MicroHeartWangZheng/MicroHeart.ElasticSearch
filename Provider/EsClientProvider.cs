using Elasticsearch.Net;
using Microsoft.Extensions.Options;
using Nest;
using System;
using System.Linq;

namespace ElasticSearch.Repository.Provider
{
    public class EsClientProvider : IEsClientProvider
    {
        private ElasticSearchOptions options;
        public EsClientProvider(IOptions<ElasticSearchOptions> options)
        {
            if (options == null)
                throw new NullReferenceException("ES未配置");
            this.options = options.Value;
        }

        public ElasticClient GetClient(string indexName)
        {
            var uris = options.ConnectionStrings.Select(x => new Uri(x));
            var connectionPool = new SniffingConnectionPool(uris);
            var connectionSetting = new ConnectionSettings(connectionPool);

            if (!string.IsNullOrWhiteSpace(indexName))
            {
                connectionSetting.DefaultIndex(indexName);
            }
            return new ElasticClient(connectionSetting);
        }
    }
}
