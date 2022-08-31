using Nest;
using System.Threading.Tasks;

namespace ElasticSearch.Repository.Extensions
{
    public static class ElasticClientExtension
    {

        public static bool CreateIndex<T>(this IElasticClient elasticClient, string indexName, int numberOfShards, int numberOfReplicas, int maxResultWindow) where T : class
        {
            if (elasticClient.Indices.Exists(indexName).Exists)
                return true;
            var indexState = new IndexState()
            {
                Settings = new IndexSettings()
                {
                    NumberOfReplicas = numberOfReplicas,
                    NumberOfShards = numberOfShards,
                },
            };
            var response = elasticClient.Indices.Create(indexName, p => p.Settings(x => x.NumberOfShards(numberOfShards)
                                                                                                   .NumberOfReplicas(numberOfReplicas)
                                                                                                   .Setting("max_result_window", maxResultWindow))
                                                                         .Map<T>(item => item.AutoMap()));
            if (!response.Acknowledged)
                throw new System.Exception(response.DebugInformation);
            return response.Acknowledged;
        }


        public static async Task<bool> CreateIndexAsync<T>(this IElasticClient elasticClient, string indexName, int numberOfShards, int numberOfReplicas, int maxResultWindow) where T : class
        {
            if (elasticClient.Indices.Exists(indexName).Exists)
                return true;

            var response = await elasticClient.Indices.CreateAsync(indexName, p => p.Settings(x => x.NumberOfShards(numberOfShards)
                                                                                                    .NumberOfReplicas(numberOfReplicas)
                                                                                                    .Setting("max_result_window", maxResultWindow))
                                                                                    .Map<T>(item => item.AutoMap()));
            if (!response.Acknowledged)
                throw new System.Exception(response.DebugInformation);
            return response.Acknowledged;
        }
    }
}
