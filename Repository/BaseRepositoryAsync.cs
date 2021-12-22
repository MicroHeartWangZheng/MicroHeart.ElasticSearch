using ElasticSearch.Repository.Enum;
using ElasticSearch.Repository.Extensions;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearch.Repository
{
    partial class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        public async Task<bool> InsertAsync(T t)
        {
            await ExistOrCreateAsync();
            var response = await client.IndexAsync(t, s => s.Index(indexName));
            return response.IsValid;
        }

        public async Task<bool> InsertManyAsync(IEnumerable<T> entities)
        {
            await ExistOrCreateAsync();

            var response = await client.IndexManyAsync(entities, indexName);
            return response.IsValid;
        }


        public async Task<bool> BulkAsync(IEnumerable<T> entities, Func<BulkIndexDescriptor<T>, T, IBulkIndexOperation<T>> bulkIndexSelector = null)
        {
            await ExistOrCreateAsync();

            var resp = await client.BulkAsync(b => b.Index(indexName).IndexMany(entities, bulkIndexSelector));
            return resp.IsValid;
        }

        public async Task<bool> DeleteAsync(Id id)
        {
            var response = await client.DeleteAsync<T>(id);
            return response.IsValid;
        }
        public async Task<bool> DeleteByQueryAsync(Func<DeleteByQueryDescriptor<T>, IDeleteByQueryRequest> selector)
        {
            var response = await client.DeleteByQueryAsync(selector);
            return response.IsValid;
        }

        public async Task<bool> UpdateAsync(Id id, T t)
        {
            var response = await client.UpdateAsync<T>(id, p => p.Doc(t));
            return response.IsValid;
        }

        public async Task<T> GetAsync(Id id)
        {
            var response = await client.GetAsync<T>(id);
            if (response.IsValid)
                return response.Source;
            return null;
        }

        public async Task<IEnumerable<T>> GetManyAsync(IEnumerable<string> ids)
        {
            var result = new List<T>();
            var response = await client.GetManyAsync<T>(ids);
            if ((response?.Count() ?? 0) == 0)
                return null;
            foreach (var item in response)
            {
                result.Add(item.Source);
            }
            return result;
        }
        public async Task<IEnumerable<T>> GetManyAsync(IEnumerable<long> ids)
        {
            var result = new List<T>();
            var response = await client.GetManyAsync<T>(ids);
            if ((response?.Count() ?? 0) == 0)
                return null;
            foreach (var item in response)
            {
                result.Add(item.Source);
            }
            return result;
        }

        public async Task<(IEnumerable<T>, long)> SearchAsync(Func<SearchDescriptor<T>, ISearchRequest> selector)
        {
            var result = new List<T>();
            var response = await client.SearchAsync(selector);
            if (response.ApiCall.RequestBodyInBytes != null)
            {
                var responseJson = System.Text.Encoding.UTF8.GetString(response.ApiCall.RequestBodyInBytes);
            }
            if (!response.IsValid)
                return (null, 0);

            foreach (var hit in response.Hits)
            {
                result.Add(hit.Source);
            }
            return (result, response.Total);
        }

        public async Task<IEnumerable<IHit<T>>> HitsSearchAsync(Func<SearchDescriptor<T>, ISearchRequest> selector)
        {
            ISearchResponse<T> response = await client.SearchAsync(selector);
            if (response.IsValid)
                return response.Hits;
            return null;
        }

        public async Task<TermsAggregate<string>> AggsSearchAsync(Func<SearchDescriptor<T>, ISearchRequest> selector, string key)
        {
            ISearchResponse<T> response = await client.SearchAsync(selector);
            if (response.IsValid)
                return response.Aggregations.Terms(key);
            return null;
        }

        public async Task<IEnumerable<string>> AnalyzeAsync(EnumAnalyzer analyzer, string text)
        {
            var response = await client.Indices.AnalyzeAsync(a => a.Analyzer(analyzer.ToDescription()).Text(text));
            if (!response.IsValid)
                throw null;
            return response.Tokens.Select(x => x.Token);
        }


        private async Task ExistOrCreateAsync()
        {
            var result = await client.Indices.ExistsAsync(indexName);
            if (result.Exists)
                return;
            var createResult = await client.CreateIndexAsync<T>(indexName);
            if (!createResult)
                throw new Exception("创建Index失败");
        }
    }
}
