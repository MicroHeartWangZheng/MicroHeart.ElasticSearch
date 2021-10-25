using Nest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearch.Repository.Extensions
{
    public static class ElasticClientExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="elasticClient"></param>
        /// <param name="indexName"></param>
        /// <param name="numberOfShards">主分片数量</param>
        /// <param name="numberOfReplicas">每个主分片的副分片</param>
        /// <returns></returns>
        public static bool CreateIndex<T>(this IElasticClient elasticClient, string indexName = "", int numberOfShards = 1, int numberOfReplicas = 1) where T : class
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


        public static async Task<bool> CreateIndexAsync<T>(this IElasticClient elasticClient, string indexName = "", int numberOfShards = 1, int numberOfReplicas = 1) where T : class
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
