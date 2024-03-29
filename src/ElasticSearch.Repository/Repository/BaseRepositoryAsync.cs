﻿using ElasticSearch.Repository.Enum;
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
        public async Task InsertAsync(T t)
        {
            await ExistOrCreateAsync();
            var response = await client.IndexAsync(t, s => s.Index(IndexName));
            DealResponse(response);
        }

        public async Task InsertManyAsync(IEnumerable<T> entities)
        {
            await ExistOrCreateAsync();
            var response = await client.IndexManyAsync(entities, IndexName);
            DealResponse(response);
        }


        public async Task BulkAsync(IEnumerable<T> entities, Func<BulkIndexDescriptor<T>, T, IBulkIndexOperation<T>> bulkIndexSelector = null)
        {
            await ExistOrCreateAsync();
            var response = await client.BulkAsync(b => b.Index(IndexName).IndexMany(entities, bulkIndexSelector));
            DealResponse(response);
        }

        public async Task DeleteAsync(Id id)
        {
            var response = await client.DeleteAsync<T>(id, x => x.Index(IndexName));
            DealResponse(response);
        }
        public async Task DeleteByQueryAsync(DeleteByQueryDescriptor<T> descriptor)
        {
            descriptor = descriptor.Index(IndexName);
            Func<DeleteByQueryDescriptor<T>, IDeleteByQueryRequest> selector = x => descriptor;
            var response = await client.DeleteByQueryAsync(selector);
            DealResponse(response);
        }

        public async Task UpdateAsync(Id id, T t)
        {
            var response = await client.UpdateAsync<T>(id, p => p.Index(IndexName).Doc(t));
            DealResponse(response);
        }

        public async Task UpdateByQueryAsync(UpdateByQueryDescriptor<T> descriptor)
        {
            descriptor = descriptor.Index(IndexName);
            Func<UpdateByQueryDescriptor<T>, IUpdateByQueryRequest> selector = x => descriptor;
            var response = await client.UpdateByQueryAsync<T>(selector);
            DealResponse(response);
        }

        public async Task<T> GetAsync(Id id)
        {
            var response = await client.GetAsync<T>(id, x => x.Index(IndexName));
            DealResponse(response);
            return response.Source;
        }

        public async Task<IEnumerable<T>> GetManyAsync(IEnumerable<string> ids)
        {
            var result = new List<T>();
            var response = await client.GetManyAsync<T>(ids, IndexName);
            if ((response?.Count() ?? 0) != 0)
                result.AddRange(response.Select(x => x.Source));
            return result;
        }
        public async Task<IEnumerable<T>> GetManyAsync(IEnumerable<long> ids)
        {
            var result = new List<T>();
            var response = await client.GetManyAsync<T>(ids, IndexName);
            if ((response?.Count() ?? 0) != 0)
                result.AddRange(response.Select(x => x.Source));
            return result;
        }

        public async Task<(IEnumerable<T>, long)> SearchAsync(SearchDescriptor<T> descriptor)
        {
            var result = new List<T>();
            descriptor = descriptor.Index(IndexName);
            Func<SearchDescriptor<T>, ISearchRequest> selector = x => descriptor;
            var response = await client.SearchAsync(selector);
            DealResponse(response);
            if (response.ApiCall.RequestBodyInBytes != null)
            {
                var responseJson = System.Text.Encoding.UTF8.GetString(response.ApiCall.RequestBodyInBytes);
            }

            result.AddRange(response.Hits.Select(x => x.Source));
            return (result, response.Total);
        }

        public async Task<IEnumerable<IHit<T>>> HitsSearchAsync(SearchDescriptor<T> descriptor)
        {
            descriptor = descriptor.Index(IndexName);
            Func<SearchDescriptor<T>, ISearchRequest> selector = x => descriptor;
            var response = await client.SearchAsync(selector);
            DealResponse(response);
            return response.Hits;
        }

        public async Task<TermsAggregate<string>> AggsSearchAsync(SearchDescriptor<T> descriptor, string key)
        {
            descriptor = descriptor.Index(IndexName);
            Func<SearchDescriptor<T>, ISearchRequest> selector = x => descriptor;
            var response = await client.SearchAsync(selector);
            DealResponse(response);
            return response.Aggregations.Terms(key);
        }

        public async Task<IEnumerable<string>> AnalyzeAsync(EnumAnalyzer analyzer, string text)
        {
            var response = await client.Indices.AnalyzeAsync(a => a.Analyzer(analyzer.ToDescription()).Text(text));
            DealResponse(response);
            return response.Tokens.Select(x => x.Token);
        }


        private async Task ExistOrCreateAsync()
        {
            await client.CreateIndexAsync<T>(IndexName,Setting);
        }

        private void DealResponse(IResponse response)
        {
            if (!response.IsValid)
                throw new Exception(response.DebugInformation);
        }
    }
}
