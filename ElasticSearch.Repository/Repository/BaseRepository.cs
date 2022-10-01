using ElasticSearch.Repository.Enum;
using ElasticSearch.Repository.Extensions;
using Microsoft.Extensions.Options;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearch.Repository
{
    public partial class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly IElasticClient client;
        private readonly ElasticSearchOptions options;

        /// <summary>
        /// 索引名称
        /// </summary>
        public virtual string IndexName { get; set; } = typeof(T).Name.ToLower();

        /// <summary>
        /// 主分片数量
        /// </summary>
        public virtual int NumberOfShards => 1;

        /// <summary>
        /// 每个主分片的副分片数量
        /// </summary>
        public virtual int NumberOfReplicas => 0;

        /// <summary>
        /// 返回最大数量 默认1000000
        /// </summary>
        public virtual int MaxResultWindow => 1000000;

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

        public void Insert(T t)
        {
            ExistOrCreate();
            var response = client.Index(t, s => s.Index(IndexName));
            DealResponse(response);
        }

        public void InsertMany(IEnumerable<T> entities)
        {
            ExistOrCreate();
            var response = client.IndexMany(entities, IndexName);
            DealResponse(response);
        }

        public void Bulk(IEnumerable<T> entities, Func<BulkIndexDescriptor<T>, T, IBulkIndexOperation<T>> bulkIndexSelector = null)
        {
            ExistOrCreate();
            var response = client.Bulk(b => b.Index(IndexName).IndexMany(entities, bulkIndexSelector));
            DealResponse(response);
        }

        public void Delete(Id id)
        {
            var response = client.Delete<T>(id, d => d.Index(IndexName));
            DealResponse(response);
        }

        public void DeleteByQuery(DeleteByQueryDescriptor<T> descriptor)
        {
            descriptor = descriptor.Index(IndexName);
            Func<DeleteByQueryDescriptor<T>, IDeleteByQueryRequest> selector = x => descriptor;
            var response = client.DeleteByQuery(selector);
            DealResponse(response);
        }

        public void Update(Id id, T t)
        {
            var response = client.Update<T>(id, p => p.Index(IndexName).Doc(t));
            DealResponse(response);
        }

        public void UpdateByQuery(UpdateByQueryDescriptor<T> descriptor)
        {
            descriptor = descriptor.Index(IndexName);
            Func<UpdateByQueryDescriptor<T>, IUpdateByQueryRequest> selector = x => descriptor;
            var response = client.UpdateByQuery<T>(selector);
            DealResponse(response);
        }

        public T Get(Id id)
        {
            var response = client.Get<T>(id, g => g.Index(IndexName));
            DealResponse(response);
            return response.Source;
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
            DealResponse(response);
            result.AddRange(response.Hits.Select(x => x.Source));
            return (result, response.Total);
        }

        public IEnumerable<IHit<T>> HitsSearch(SearchDescriptor<T> descriptor)
        {
            descriptor = descriptor.Index(IndexName);
            Func<SearchDescriptor<T>, ISearchRequest> selector = x => descriptor;
            var response = client.Search(selector);
            DealResponse(response);
            return null;
        }

        public IEnumerable<string> Analyze(EnumAnalyzer analyzer, string text)
        {
            var response = client.Indices.Analyze(a => a.Analyzer(analyzer.ToDescription()).Text(text));
            DealResponse(response);
            return response.Tokens.Select(x => x.Token);
        }

        private void ExistOrCreate()
        {
            client.CreateIndex<T>(IndexName, NumberOfShards, NumberOfReplicas, MaxResultWindow);
        }
    }
}
