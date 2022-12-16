using ElasticSearch.Repository;
using Microsoft.Extensions.Options;
using Nest;

namespace ElasticSearch.Example.Repository
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        //可以重写索引的名称，默认是以 配置项的前缀+实体类名称小写 作为索引名称
        //public override string IndexName =>"goods"  

        //对索引的设置   
        // 1、如果不重写  下面是默认设置
        public override Func<IndexSettingsDescriptor, IPromise<IIndexSettings>> Setting => x => x.NumberOfShards(1)
                                                                                                 .NumberOfReplicas(0)
                                                                                                 .Setting("max_result_window", 1000000);
        //2、 重写  自定义索引设置
        //public override Func<IndexSettingsDescriptor, IPromise<IIndexSettings>> Setting => x => x.NumberOfShards(1)
        //                                                                       .NumberOfReplicas(0)
        //                                                                       .Setting("max_result_window", 1000000)
        //                                                                       .Analysis(a => a
        //                                                                              .TokenFilters(tf => tf
        //                                                                                      .Synonym("synonym_filter", s => s
        //                                                                                              .SynonymsPath("analysis/synonym.txt")))
        //                                                                              .Analyzers(an => an
        //                                                                                      .Custom("synonym_ik_max_word", ca => ca
        //                                                                                              .Tokenizer("ik_max_word")
        //                                                                                              .Filters("synonym_filter"))
        //                                                                                      .Custom("synonym_ik_smart", ca => ca
        //                                                                                                      .Tokenizer("ik_smart")
        //                                                                                                      .Filters("synonym_filter"))));
        public ProductRepository(IElasticClient client, IOptions<ElasticSearchOptions> options) : base(client, options)
        {
        }
    }
}
