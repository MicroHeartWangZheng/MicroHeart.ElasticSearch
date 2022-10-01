using ElasticSearch.Repository.Enum;
using Nest;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElasticSearch.Repository
{
    public interface IBaseRepository<T> where T : class
    {
        void Insert(T t);
        Task InsertAsync(T t);

        void InsertMany(IEnumerable<T> entities);
        Task InsertManyAsync(IEnumerable<T> entities);

        void Bulk(IEnumerable<T> entities, Func<BulkIndexDescriptor<T>, T, IBulkIndexOperation<T>> bulkIndexSelector = null);
        Task BulkAsync(IEnumerable<T> entities, Func<BulkIndexDescriptor<T>, T, IBulkIndexOperation<T>> bulkIndexSelector = null);

        void Delete(Id id);
        Task DeleteAsync(Id id);

        void DeleteByQuery(DeleteByQueryDescriptor<T> descriptor);
        Task DeleteByQueryAsync(DeleteByQueryDescriptor<T> descriptor);

        void Update(Id id, T t);
        Task UpdateAsync(Id id, T t);

        void UpdateByQuery(UpdateByQueryDescriptor<T> descriptor);
        Task UpdateByQueryAsync(UpdateByQueryDescriptor<T> descriptor);

        T Get(Id id);
        Task<T> GetAsync(Id id);

        IEnumerable<T> GetMany(IEnumerable<string> ids);
        Task<IEnumerable<T>> GetManyAsync(IEnumerable<string> ids);

        IEnumerable<T> GetMany(IEnumerable<long> ids);
        Task<IEnumerable<T>> GetManyAsync(IEnumerable<long> ids);

        (IEnumerable<T>, long) Search(SearchDescriptor<T> descriptor);
        Task<(IEnumerable<T>, long)> SearchAsync(SearchDescriptor<T> descriptor);

        IEnumerable<IHit<T>> HitsSearch(SearchDescriptor<T> descriptor);
        Task<IEnumerable<IHit<T>>> HitsSearchAsync(SearchDescriptor<T> descriptor);

        Task<TermsAggregate<string>> AggsSearchAsync(SearchDescriptor<T> descriptor, string key);

        IEnumerable<string> Analyze(EnumAnalyzer analyzer, string text);
        Task<IEnumerable<string>> AnalyzeAsync(EnumAnalyzer analyzer, string text);
    }
}
