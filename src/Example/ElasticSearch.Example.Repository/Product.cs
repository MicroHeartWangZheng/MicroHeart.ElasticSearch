using Nest;

namespace ElasticSearch.Example.Repository
{
    public class Product
    {
        [Number(NumberType.Long)]
        public long Id { get; set; }

        [Text(Analyzer = "ik_max_word")]
        public string Name { get; set; }

        [Text(Analyzer = "ik_max_word")]
        public string Description { get; set; }
    }
}