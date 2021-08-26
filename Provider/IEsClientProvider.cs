using Nest;

namespace ElasticSearch.Repository.Provider
{
    public interface IEsClientProvider
    {
        ElasticClient GetClient(string indexName);
    }
}
