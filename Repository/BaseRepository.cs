using ElasticSearch.Repository.Enum;
using ElasticSearch.Repository.Extensions;
using ElasticSearch.Repository.Provider;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ElasticSearch.Repository
{
    public partial class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        public readonly IElasticClient client;

        public virtual string indexName => "";

        public BaseRepository(IElasticClient client)
        {
            this.client = client;
        }

        public bool Insert(T t)
        {
            if (!client.Indices.Exists(indexName).Exists)
            {
                var result = client.CreateIndex<T>(indexName);
                if (!result)
                    throw new Exception("创建Index失败");
            }

            return client.Index(t, s => s.Index(indexName)).IsValid;
        }

        public bool InsertMany(IEnumerable<T> entities)
        {
            if (!client.Indices.Exists(indexName).Exists)
            {
                var result = client.CreateIndex<T>(indexName);
                if (!result)
                    throw new Exception("创建Index失败");
            }

            return client.IndexMany(entities, indexName).IsValid;
        }

        public bool Bulk(IEnumerable<T> entities, Func<BulkIndexDescriptor<T>, T, IBulkIndexOperation<T>> bulkIndexSelector = null)
        {
            if (!client.Indices.Exists(indexName).Exists)
            {
                var result = client.CreateIndex<T>(indexName);
                if (!result)
                    throw new Exception("创建Index失败");
            }

            return client.Bulk(b => b.Index(indexName).IndexMany(entities, bulkIndexSelector)).IsValid;
        }

        public bool Delete(Id id)
        {
            return client.Delete<T>(id, d => d.Index(indexName)).IsValid;
        }

        public bool DeleteByQuery(Func<DeleteByQueryDescriptor<T>, IDeleteByQueryRequest> selector)
        {
            return client.DeleteByQuery(selector).IsValid;
        }

        public bool Update(Id id, T t)
        {
            return client.Update<T>(id, p => p.Index(indexName).Doc(t)).IsValid;
        }

        public T Get(Id id)
        {
            var response = client.Get<T>(id, g => g.Index(indexName));
            if (response.IsValid && response.Found)
                return response.Source;
            return null;
        }

        public IEnumerable<T> GetMany(IEnumerable<string> ids)
        {
            var result = new List<T>();
            var response = client.GetMany<T>(ids, indexName);
            if ((response?.Count() ?? 0) == 0)
                return null;
            foreach (var item in response)
            {
                result.Add(item.Source);
            }
            return result;
        }

        public IEnumerable<T> GetMany(IEnumerable<long> ids)
        {
            var result = new List<T>();
            var response = client.GetMany<T>(ids, indexName);
            if ((response?.Count() ?? 0) == 0)
                return null;
            foreach (var item in response)
            {
                result.Add(item.Source);
            }
            return result;
        }

        public (IEnumerable<T>, long) Search(Func<SearchDescriptor<T>, ISearchRequest> selector)
        {
            var result = new List<T>();
            var response = client.Search(selector);
            if (!response.IsValid)
                return (null, 0);

            foreach (var hit in response.Hits)
            {
                result.Add(hit.Source);
            }
            return (result, response.Total);
        }

        public IEnumerable<IHit<T>> HitsSearch(Func<SearchDescriptor<T>, ISearchRequest> selector)
        {
            var response = client.Search(selector);
            if (response.IsValid)
                return response.Hits;
            return null;
        }

        public IEnumerable<string> Analyze(EnumAnalyzer analyzer, string text)
        {
            var response = client.Indices.Analyze(a => a.Analyzer(analyzer.ToDescription()).Text(text));
            if (!response.IsValid)
                throw null;
            return response.Tokens.Select(x => x.Token);
        }
    }
}
