using Nest;

namespace ElasticSearch.Repository.Provider
{
    /// <summary>
    /// 不用单例模式实现 使用IOC单例注入实现
    /// </summary>
    public interface IEsClientProvider
    {
        ElasticClient GetClient(string indexName);
    }
}
