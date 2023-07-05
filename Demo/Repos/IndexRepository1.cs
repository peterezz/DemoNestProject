//using Elasticsearch.Net;
//using Nest;

//namespace iucon.Onix.ElasticSearch.Repositories
//{
//    public class IndexRepository<T>
//    {
//        private readonly ElasticClient _elasticClient;
//        //private readonly TDocument _document;
//        public IndexRepository ( ElasticClient elasticClient )
//        {
//            _elasticClient = elasticClient;

//            // _document = document;

//        }

//        public async Task<IEnumerable<string>> Exists ( IEnumerable<string> keys )
//        {
//            var getResponse = await _elasticClient.SearchAsync<string>(d => d.Query(q => q.Ids(i => i.Values(keys))).Source(false).Size(keys.Count()));
//            if (getResponse.IsValid)
//                return getResponse.Hits.Select(h => h.Id);
//            else
//                return null;
//        }

//        public async Task<T ReadAsync (string key )
//        {
//            var getResponse = await _elasticClient.SearchAsync<>;
//            if (getResponse.IsValid)
//                return getResponse.Source;
//            else
//                return null;
//        }

//    public async Task<IEnumerable<T>> ReadBulk ( IEnumerable<string> keys )
//    {
//        var getResponse = await _elasticClient.GetManyAsync<T>(keys);
//        return getResponse.Where(p => p.Source != null).Select(p => p.Source).Cast<T>();
//    }

//    public async Task<bool> Update ( string key, T data )
//    {
//        AddTimestamp(new T[ ] { data });

//        UpdateResponse<T>? updateResult = null;
//        CreateResponse? createResult = null;

//        var doc = await ReadAsync(key);
//        if (doc != null)
//            updateResult = await _elasticClient.UpdateAsync<T>(key, ud => ud.Doc(data));
//        else
//            createResult = await _elasticClient.CreateDocumentAsync(data);

//        return (updateResult?.Result != null && updateResult?.Result != Result.Error)
//            || (createResult?.Result != null && createResult?.Result != Result.Error);
//    }

//    public async Task<bool> UpdateBulk ( IEnumerable<T> data )
//    {
//        AddTimestamp(data);
//        var descr = new BulkDescriptor();
//        descr.IndexMany<T>(data);

//        var result = await _elasticClient.BulkAsync(descr);
//        return !result.Errors;
//    }

//    public Task<ISearchResponse<T>> SearchAsync<TRequest> ( SearchDescriptor<TRequest> request ) where TRequest : class
//        => _elasticClient.SearchAsync<T>(request);

//    public async Task<bool> DeleteDocument ( T data )
//    {
//        var x = new DeleteDescriptor<T>(data).Refresh(Refresh.True);
//        var result = await _elasticClient.DeleteAsync(x);
//        return result.IsValid;
//    }

//    public async Task<bool> DeleteDocument ( string id )
//    {
//        var x = new DeleteDescriptor<T>(id).Refresh(Refresh.True);
//        var result = await _elasticClient.DeleteAsync(x);
//        return result.IsValid;
//    }

//    public async Task<bool> DeleteDocumentsBulk ( IEnumerable<string> ids )
//    {
//        var descriptor = new DeleteByQueryDescriptor<T>();
//        var result = await _elasticClient.DeleteByQueryAsync<T>(d => d.Query(q => q.Ids(i => i.Values(ids))).Refresh(true).WaitForCompletion());
//        return result.IsValid;
//    }

//    public async Task<bool> DeleteIndex ( string idx )
//    {
//        var result = await _elasticClient.Indices.DeleteAsync(idx);
//        return result.IsValid;
//    }

//    public async Task<bool> Reindex ( string source, string destination, int slices )
//    {
//        var result = await _elasticClient.ReindexOnServerAsync(ri
//            => ri.Source(s => s.Index(source))
//                 .Destination(d => d.Index(destination))
//                 .Slices(slices)
//                 .Refresh(true)
//                 .WaitForCompletion(true));

//        return result.IsValid;
//    }

//    public async Task<bool> UpdateMapping ( string indexName )
//        => (await _elasticClient.UpdateIndexMapping<T>(indexName)).IsValid;

//    public async Task<IndexResponse> IndexDocument ( )
//    {
//        var documentResponse = await _elasticClient.IndexDocumentAsync();
//        if (documentResponse.IsValid)
//            Console.WriteLine("document added successfully \n");
//        Console.WriteLine(documentResponse.ServerError.ToString());
//        return documentResponse;

//    }

//    public async Task BulkCreate ( TDocument[ ] DocumentArray, string indexName ) where TDocument : class
//    {
//        var indexManyResponse = await _elasticClient.BulkAsync(b => b.Index(indexName).IndexMany(DocumentArray));
//        if (indexManyResponse.IsValid)
//            Console.WriteLine("Bulk Create requested successfully \n");
//        Console.WriteLine(indexManyResponse.ServerError.ToString());

//    }
//    #region Private methods

//    private void AddTimestamp ( IEnumerable<T> data )
//    {
//        foreach (var item in data)
//            ((IElasticEntity)item).Timestamp = DateTime.Now;
//    }



//    #endregion Private methods
//}
//}
