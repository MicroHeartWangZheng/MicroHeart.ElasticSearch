using Nest;
using System.Threading.Tasks;

namespace ElasticSearch.Repository.Extensions
{
    public static class ElasticClientExtension
    {

        public static void CreateIndex<T>(this IElasticClient elasticClient, string indexName, int numberOfShards, int numberOfReplicas, int maxResultWindow) where T : class
        {
            if (elasticClient.Indices.Exists(indexName).Exists)
                return;

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
            if (!response.IsValid)
                throw new System.Exception(response.DebugInformation);
        }


        public static async Task CreateIndexAsync<T>(this IElasticClient elasticClient, string indexName, int numberOfShards, int numberOfReplicas, int maxResultWindow) where T : class
        {
            if (elasticClient.Indices.Exists(indexName).Exists)
                return;

            var response = await elasticClient.Indices.CreateAsync(indexName, p => p.Settings(x => x.NumberOfShards(numberOfShards)
                                                                                                    .NumberOfReplicas(numberOfReplicas)
                                                                                                    .Setting("max_result_window", maxResultWindow))
                                                                                    .Map<T>(item => item.AutoMap()));
            if (!response.IsValid)
                throw new System.Exception(response.DebugInformation);
        }
    }
}
