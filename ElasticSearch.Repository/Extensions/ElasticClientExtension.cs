using Nest;
using System;
using System.Threading.Tasks;

namespace ElasticSearch.Repository.Extensions
{
    public static class ElasticClientExtension
    {

        public static void CreateIndex<T>(this IElasticClient elasticClient, string indexName, Func<IndexSettingsDescriptor, IPromise<IIndexSettings>> setting) where T : class
        {
            if (elasticClient.Indices.Exists(indexName).Exists)
                return;

            if (setting == null)
                setting = x => x.NumberOfShards(1).NumberOfReplicas(0).Setting("max_result_window", 1000000);

            var response = elasticClient.Indices.Create(indexName, p => p.Settings(setting)
                                                                       .Map<T>(item => item.AutoMap()));

            if (!response.IsValid)
                throw new Exception(response.DebugInformation);
        }


        public static async Task CreateIndexAsync<T>(this IElasticClient elasticClient, string indexName, Func<IndexSettingsDescriptor, IPromise<IIndexSettings>> setting) where T : class
        {
            var exist = await elasticClient.Indices.ExistsAsync(indexName);
            if (exist.Exists)
                return;

            if (setting == null)
                setting = x => x.NumberOfShards(1).NumberOfReplicas(0).Setting("max_result_window", 1000000);

            var response = await elasticClient.Indices.CreateAsync(indexName, p => p.Settings(setting)
                                                                                    .Map<T>(item => item.AutoMap()));
            if (!response.IsValid)
                throw new Exception(response.DebugInformation);
        }
    }
}
