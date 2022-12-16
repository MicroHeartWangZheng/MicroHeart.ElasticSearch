using System.Collections.Generic;

namespace ElasticSearch.Repository
{
    public class ElasticSearchOptions
    {
        public List<string> ConnectionStrings { get; set; }

        /// <summary>
        /// 索引前缀(防止索引名称重复)
        /// </summary>
        public string IndexPrefix { get; set; }

        /// <summary>
        /// 禁用直流式传输
        /// </summary>
        public bool DisableDirectStreaming { get; set; }
    }
}
