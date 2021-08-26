using Nest;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElasticSearch.Repository
{
    public interface IBaseRepository<T> where T : class
    {
        bool Insert(T t);
        Task<bool> InsertAsync(T t);

        bool InsertMany(IEnumerable<T> entitys);
        Task<bool> InsertManyAsync(IEnumerable<T> entitys);

        bool Delete(Id id);
        Task<bool> DeleteAsync(Id id);

        bool Update(Id id, T t);
        Task<bool> UpdateAsync(Id id, T t);

        T Get(Id id);
        Task<T> GetAsync(Id id);

        IEnumerable<T> GetMany(IEnumerable<string> ids);
        Task<IEnumerable<T>> GetManyAsync(IEnumerable<string> ids);

        IEnumerable<T> GetMany(IEnumerable<long> ids);
        Task<IEnumerable<T>> GetManyAsync(IEnumerable<long> ids);

        IEnumerable<T> Search(ISearchRequest request);
        Task<IEnumerable<T>> SearchAsync(ISearchRequest request);

        IEnumerable<T> Search(Func<SearchDescriptor<T>, ISearchRequest> selector);
        Task<IEnumerable<T>> SearchAsync(Func<SearchDescriptor<T>, ISearchRequest> selector);

        IEnumerable<IHit<T>> HitsSearch(ISearchRequest request);
        Task<IEnumerable<IHit<T>>> HitsSearchAsync(ISearchRequest request);

        IEnumerable<IHit<T>> HitsSearch(Func<SearchDescriptor<T>, ISearchRequest> selector);
        Task<IEnumerable<IHit<T>>> HitsSearchAsync(Func<SearchDescriptor<T>, ISearchRequest> selector);
    }
}
