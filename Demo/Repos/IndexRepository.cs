using Nest;

namespace iucon.Onix.ElasticSearch.Repositories
{
    public class IndexRepository<TDocument> where TDocument : class
    {
        private readonly ElasticClient _elasticClient;

        public IndexRepository ( ElasticClient elasticClient )
        {
            _elasticClient = elasticClient;

        }
        #region IUCON Requests


        //public async Task<IEnumerable<string>> Exists ( IEnumerable<string> keys )
        //{
        //    var getResponse = await _elasticClient.SearchAsync<string>(d => d.Query(q => q.Ids(i => i.Values(keys))).Source(false).Size(keys.Count()));
        //    if (getResponse.IsValid)
        //        return getResponse.Hits.Select(h => h.Id);
        //    else
        //        return null;
        //}

        //public async Task<T> ReadAsync ( string key )
        //{
        //    var getResponse = await _elasticClient.GetAsync<T>(key);
        //    if (getResponse.IsValid)
        //        return getResponse.Source;
        //    else
        //        return null;
        //}

        //public async Task<IEnumerable<T>> ReadBulk ( IEnumerable<string> keys )
        //{
        //    var getResponse = await _elasticClient.GetManyAsync<T>(keys);
        //    return getResponse.Where(p => p.Source != null).Select(p => p.Source).Cast<T>();
        //}

        //public async Task<bool> Update ( string key, T data )
        //{
        //    AddTimestamp(new T[ ] { data });

        //    UpdateResponse<T>? updateResult = null;
        //    CreateResponse? createResult = null;

        //    var doc = await ReadAsync(key);
        //    if (doc != null)
        //        updateResult = await _elasticClient.UpdateAsync<T>(key, ud => ud.Doc(data));
        //    else
        //        createResult = await _elasticClient.CreateDocumentAsync(data);

        //    return (updateResult?.Result != null && updateResult?.Result != Result.Error)
        //        || (createResult?.Result != null && createResult?.Result != Result.Error);
        //}

        //public async Task<bool> UpdateBulk ( IEnumerable<T> data )
        //{
        //    AddTimestamp(data);
        //    var descr = new BulkDescriptor();
        //    descr.IndexMany<T>(data);

        //    var result = await _elasticClient.BulkAsync(descr);
        //    return !result.Errors;
        //}

        //public Task<ISearchResponse<T>> SearchAsync<TRequest> ( SearchDescriptor<TRequest> request ) where TRequest : class
        //    => _elasticClient.SearchAsync<T>(request);

        //public async Task<bool> DeleteDocument ( T data )
        //{
        //    var x = new DeleteDescriptor<T>(data).Refresh(Refresh.True);
        //    var result = await _elasticClient.DeleteAsync(x);
        //    return result.IsValid;
        //}

        //public async Task<bool> DeleteDocument ( string id )
        //{
        //    var x = new DeleteDescriptor<T>(id).Refresh(Refresh.True);
        //    var result = await _elasticClient.DeleteAsync(x);
        //    return result.IsValid;
        //}

        //public async Task<bool> DeleteDocumentsBulk ( IEnumerable<string> ids )
        //{
        //    var descriptor = new DeleteByQueryDescriptor<T>();
        //    var result = await _elasticClient.DeleteByQueryAsync<T>(d => d.Query(q => q.Ids(i => i.Values(ids))).Refresh(true).WaitForCompletion());
        //    return result.IsValid;
        //}

        //public async Task<bool> DeleteIndex ( string idx )
        //{
        //    var result = await _elasticClient.Indices.DeleteAsync(idx);
        //    return result.IsValid;
        //}

        //public async Task<bool> Reindex ( string source, string destination, int slices )
        //{
        //    var result = await _elasticClient.ReindexOnServerAsync(ri
        //        => ri.Source(s => s.Index(source))
        //             .Destination(d => d.Index(destination))
        //             .Slices(slices)
        //             .Refresh(true)
        //             .WaitForCompletion(true));

        //    return result.IsValid;
        //}

        //public async Task<bool> UpdateMapping ( string indexName )
        //    => (await _elasticClient.UpdateIndexMapping<T>(indexName)).IsValid;
        #region Private methods

        //private void AddTimestamp ( IEnumerable<T> data )
        //{
        //    foreach (var item in data)
        //        ((IElasticEntity)item).Timestamp = DateTime.Now;
        //}



        #endregion Private methods 
        #endregion

        #region MY Requests
        public async Task<IndexResponse> IndexDocument ( TDocument document )
        {
            var documentResponse = await _elasticClient.IndexDocumentAsync(document);
            if (documentResponse.IsValid)
                Console.WriteLine("document added successfully \n");
            else
                Console.WriteLine("Error Occuared");
            return documentResponse;

        }

        public async Task<BulkResponse> BulkCreate ( TDocument[ ] DocumentArray, string indexName )
        {
            var indexManyResponse = await _elasticClient.BulkAsync(b => b.Index(indexName).IndexMany(DocumentArray));
            if (!indexManyResponse.Errors)
                Console.WriteLine("Bulk Create requested successfully \n -------------\n documents \n------------------------------");
            else
                Console.WriteLine("Error Occuared");

            return indexManyResponse;

        }

        public async Task<ISearchResponse<TDocument>> MatchAllQuery ( )
        {
            var searchResponse = await _elasticClient.SearchAsync<TDocument>(s => s
                .Query(q => q
                    .MatchAll()
                )
            );
            return searchResponse;

        }
        public async Task<ISearchResponse<TDocument>> BoolQuery ( string must, string filter = null, int price = 0, string mustnot = null )
        {

            return await _elasticClient.SearchAsync<TDocument>(s => s
            .Query(q => q
                .Bool(b => b
                 //.Must(b => b
                 //    .Match(t => t.Field("productReference").Query(must))

                 //)
                 //.Must(f => f
                 //    .Term(t => t.ProductReference, must)

                 //)
                 .Should(

                     s => s.Range(r => r.Field("price").GreaterThan(4060))
                 )
                 )
             )
            .Size(100));
        }

        //aggregation
        public async Task<ISearchResponse<TDocument>> PreformSumAggregation ( string fieldName )
        {
            return await _elasticClient.SearchAsync<TDocument>(s => s
                 .Aggregations(a => a
                     .Sum("total_price", sa => sa
                       .Field(fieldName)
                   )
                  )
                );

        }
        public async Task<ISearchResponse<TDocument>> PerformFilterAggregation ( string fieldName )
        {
            return await _elasticClient.SearchAsync<TDocument>(s => s.Size(0)
            .Aggregations(a => a.Filters("filters_buckets", f => f
            .OtherBucket()
            .OtherBucketKey("Other state bucket")
            .NamedFilters(fillter => fillter

            .Filter("Product refrances starts with 14....", f => f.Wildcard(m => m.Field(fieldName).Value("14*")))
            .Filter("Product refrances starts with 15....", f => f.Wildcard(w => w.Field(fieldName).Value("15*")))).Aggregations(aa => aa
                .TopHits("my_docs", tha => tha
                    .Size(10)   // limit to 10 documents per bucket
                )))

            ));
        }

        #endregion
    }
}
