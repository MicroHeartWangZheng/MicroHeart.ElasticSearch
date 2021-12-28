using System.Collections.Generic;

namespace ElasticSearch.Repository
{
    public class ElasticSearchOptions
    {
        public List<string> ConnectionStrings { get; set; }

        /// <summary>
        /// 主分片数量
        /// </summary>
        public int NumberOfShards { get; set; }

        /// <summary>
        /// 每个主分片的副分片数量
        /// </summary>
        public int NumberOfReplicas { get; set; }
    }
}
