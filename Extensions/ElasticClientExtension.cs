using Nest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearch.Repository.Extensions
{
    public static class ElasticClientExtension
    {
        public static bool CreateIndex<T>(this ElasticClient elasticClient, string indexName = "", int numberOfShards = 5, int numberOfReplicas = 1) where T : class
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
            return elasticClient.Indices.Create(indexName, p => p.InitializeUsing(indexState).Map<T>(item => item.AutoMap())).Acknowledged;
        }


        public static async Task<bool> CreateIndexAsync<T>(this ElasticClient elasticClient, string indexName = "", int numberOfShards = 5, int numberOfReplicas = 1) where T : class
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
            var response = await elasticClient.Indices.CreateAsync(indexName, p => p.InitializeUsing(indexState).Map<T>(item => item.AutoMap()));
            return response.Acknowledged;
        }
    }
}
