using ElasticSearch.Repository.Enum;
using ElasticSearch.Repository.Extensions;
using Microsoft.Extensions.Options;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ElasticSearch.Repository
{
    public partial class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly IElasticClient client;
        private readonly ElasticSearchOptions options;
        private string IndexName { get; set; }

        public BaseRepository(IElasticClient client, IOptions<ElasticSearchOptions> options)
        {
            this.client = client;
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            this.options = options.Value;

            if (!string.IsNullOrEmpty(this.options.IndexPrefix))
                IndexName = $"{this.options.IndexPrefix.ToLower()}_{typeof(T).Name.ToLower()}";
            else
                IndexName = typeof(T).Name.ToLower();
        }

        public bool Insert(T t)
        {
            ExistOrCreate();
            return client.Index(t, s => s.Index(IndexName)).IsValid;
        }

        public bool InsertMany(IEnumerable<T> entities)
        {
            ExistOrCreate();
            return client.IndexMany(entities, IndexName).IsValid;
        }

        public bool Bulk(IEnumerable<T> entities, Func<BulkIndexDescriptor<T>, T, IBulkIndexOperation<T>> bulkIndexSelector = null)
        {
            ExistOrCreate();
            return client.Bulk(b => b.Index(IndexName).IndexMany(entities, bulkIndexSelector)).IsValid;
        }

        public bool Delete(Id id)
        {
            return client.Delete<T>(id, d => d.Index(IndexName)).IsValid;
        }

        public bool DeleteByQuery(DeleteByQueryDescriptor<T> descriptor)
        {
            descriptor = descriptor.Index(IndexName);
            Func<DeleteByQueryDescriptor<T>, IDeleteByQueryRequest> selector = x => descriptor;
            return client.DeleteByQuery(selector).IsValid;
        }

        public bool Update(Id id, T t)
        {
            return client.Update<T>(id, p => p.Index(IndexName).Doc(t)).IsValid;
        }

        public bool UpdateByQuery(UpdateByQueryDescriptor<T> descriptor)
        {
            descriptor = descriptor.Index(IndexName);
            Func<UpdateByQueryDescriptor<T>, IUpdateByQueryRequest> selector = x => descriptor;
            return client.UpdateByQuery<T>(selector).IsValid;
        }

        public T Get(Id id)
        {
            var response = client.Get<T>(id, g => g.Index(IndexName));
            if (response.IsValid && response.Found)
                return response.Source;
            return null;
        }

        public IEnumerable<T> GetMany(IEnumerable<string> ids)
        {
            var result = new List<T>();
            var response = client.GetMany<T>(ids, IndexName);
            if ((response?.Count() ?? 0) != 0)
                result.AddRange(response.Select(x => x.Source));
            return result;
        }

        public IEnumerable<T> GetMany(IEnumerable<long> ids)
        {
            var result = new List<T>();
            var response = client.GetMany<T>(ids, IndexName);
            if ((response?.Count() ?? 0) != 0)
                result.AddRange(response.Select(x => x.Source));
            return result;
        }

        public (IEnumerable<T>, long) Search(SearchDescriptor<T> descriptor)
        {
            var result = new List<T>();
            descriptor = descriptor.Index(IndexName);
            Func<SearchDescriptor<T>, ISearchRequest> selector = x => descriptor;
            var response = client.Search(selector);
            if (!response.IsValid)
                return (null, 0);
            result.AddRange(response.Hits.Select(x => x.Source));
            return (result, response.Total);
        }

        public IEnumerable<IHit<T>> HitsSearch(SearchDescriptor<T> descriptor)
        {
            descriptor = descriptor.Index(IndexName);
            Func<SearchDescriptor<T>, ISearchRequest> selector = x => descriptor;
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

        private void ExistOrCreate()
        {
            if (client.Indices.Exists(IndexName).Exists)
                return;
            if (!client.CreateIndex<T>(IndexName, options.NumberOfShards, options.NumberOfReplicas))
                throw new Exception("创建Index失败");
        }
    }
}
