using Demo.Configuration;
using Demo.Models;
using Elasticsearch.Net;
//using iucon.Onix.ElasticSearch.Extensions;
using Nest;
using Nest.JsonNetSerializer;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Net.Http.Headers;
using System.Text;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace iucon.Onix.ElasticSearch.Builders
{
    public static class ClientBuilder
    {
        private static ConcurrentDictionary<string, ILogger> indexLoggers = new ConcurrentDictionary<string, ILogger>();

        public static ElasticClient CreateIndexIfNotExistsAsync<TMap> ( string indexName, ConnectionSettings settings )
            where TMap : class
        {
            //var client = new ElasticClient();
            var client = CreateClient(settings);
            if (!client.Indices.Exists(indexName).Exists)
            {
                Console.WriteLine("index doesn't exist");
                Console.WriteLine("creating index...");

                SendCreateIndexRequest(client, indexName);
            }
            else
                PrintIndexMapping(client, indexName);

            return client;
        }

        #region NO Refrance
        public static async Task<ElasticClient> CreateIndexIfNotExistsFromMappingAsync ( ElasticsearchConfig esconfig, ConnectionSettings settings )
        {
            var client = new ElasticClient(settings);
            if (!client.Indices.Exists(esconfig.Node.IndexName).Exists)
            {
                Console.WriteLine("Index doesn't exists");
                await CreateIndexFromMappingAsync(esconfig);
            }
            Console.WriteLine("Index exists");
            return client;
        }
        #endregion


        #region NO NEED
        private static async Task<string> CreateIndexFromMappingAsync ( ElasticsearchConfig esconfig )
        {
            if (esconfig.Node == null)
                throw new Exception("Can not find profile configuration.");
            else if (!esconfig.Node.AutoMap && string.IsNullOrEmpty(esconfig.Node.Mapping?.MappingFilePath))
                throw new Exception("No mapping file is configured for the current profile.");
            else if (!File.Exists(esconfig.Node.Mapping?.MappingFilePath))
                throw new Exception("Mapping file does not exist.");

            var node = esconfig.Node;
            var mappingFile = new FileInfo(node.Mapping.MappingFilePath!);
            var mapping = await File.ReadAllTextAsync(mappingFile.FullName);
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = ( sender, cert, chain, sslPolicyErrors ) => { return true; };

            // Pass the handler to httpclient(from you are calling api)
            HttpClient client = new HttpClient(clientHandler);
            client.BaseAddress = new Uri(node.Url!);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.ConnectionClose = true;

            var authenticationString = $"{node.UserName}:{node.Password}";
            var basicAuthentication = Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));
            string createResult = await SendCreateIndexRequest(node, mapping, client, basicAuthentication);
            //string mappingResult = await SendMappingRequest(node, mapping, client, basicAuthentication);

            return JsonConvert.SerializeObject(new { CreateIndex = createResult });
        }
        #endregion

        private static void SendCreateIndexRequest ( ElasticClient client, string indexName )
        {
            var createIndexResponse = client.Indices.Create(indexName, c => c
            .Settings(s => s
                .Analysis(a => a
                    .Analyzers(an => an
                        .Standard("my_analyzer", sa => sa
                            .StopWords("_english_")
                        )
                    )
                )
            )
            .Map<demoindex>(mm => mm.Properties(p => p
            .Keyword(k => k
                .Name(n => n.ProductReference)

            )
            .Text(t => t
            .Name(n => n.ProductName)
            .Analyzer("my_analyzer")
            )
            .Number(n => n
            .Name(n => n.Price)
            .Type(NumberType.Integer)
            )
           )
          )
         );



            if (createIndexResponse.IsValid)
            {
                Console.WriteLine("Index created successfully!");

            }
            else
            {
                Console.WriteLine("Failed to create index: " + createIndexResponse.ServerError.Error);
            }
        }


        #region NO NEED
        private static async Task<string> SendCreateIndexRequest ( Demo.Configuration.MyNode node, string mapping, HttpClient client, string basicAuthentication )
        {
            var createUri = new Uri(node.Url);
            createUri = new Uri(createUri, node.IndexName);

            var createRequest = new HttpRequestMessage(System.Net.Http.HttpMethod.Put, createUri);
            createRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            createRequest.Headers.Authorization = new AuthenticationHeaderValue("Basic", basicAuthentication);
            createRequest.Content = new StringContent(mapping, Encoding.UTF8, "application/json");
            var createResponse = await client.SendAsync(createRequest);
            createResponse.EnsureSuccessStatusCode();
            var createResult = await createResponse.Content.ReadAsStringAsync();

            return createResult;
        }
        #endregion


        #region NO Refrance

        private static async Task<string> SendMappingRequest ( Demo.Configuration.MyNode node, string mapping, HttpClient client, string basicAuthentication )
        {
            var mappingUri = new Uri(node.Url);
            mappingUri = new Uri(mappingUri, $"{node.IndexName}");

            var mappingRequest = new HttpRequestMessage(System.Net.Http.HttpMethod.Put, mappingUri);
            mappingRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            mappingRequest.Headers.Authorization = new AuthenticationHeaderValue("Basic", basicAuthentication);

            mappingRequest.Content = new StringContent(mapping, Encoding.UTF8);
            mappingRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var mappingResponse = await client.SendAsync(mappingRequest);
            mappingResponse.EnsureSuccessStatusCode();
            var mappingResult = await mappingResponse.Content.ReadAsStringAsync();
            return mappingResult;
        }
        #endregion


        public static ElasticClient CreateClient ( ConnectionSettings settings )
                  => new ElasticClient(settings);

        public static ConnectionSettings BuildConnectionSettings<TMap> ( ElasticsearchConfig esconfig )
            where TMap : class
        {
            var esUri = new Uri(esconfig.Node.Url);
            var pool = new SingleNodeConnectionPool(esUri);
            var settings = new ConnectionSettings(pool, sourceSerializer: ( builtin, settings ) =>
            {
                return new JsonNetSerializer(builtin, settings,
                       ( ) => new JsonSerializerSettings
                       {
                           NullValueHandling = NullValueHandling.Ignore,
                           ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                           Formatting = Formatting.Indented
                       }
                                            );
            })
                .DefaultIndex(esconfig.Node.IndexName)
                .DefaultMappingFor<TMap>(m => m.IndexName(esconfig.Node.IndexName))
                .PrettyJson(true)
                .DisableDirectStreaming(true)
                .BasicAuthentication(esconfig.Node.UserName, esconfig.Node.Password)
                .ServerCertificateValidationCallback(CertificateValidations.AllowAll);
            //.OnRequestCompleted(details => EsLogRequest(esconfig, details));
            Console.WriteLine("Connection established successfully \n");
            return settings;
        }

        private static void PrintIndexMapping ( ElasticClient client, string indexName )
        {
            var mappingResponse = client.Indices.GetMapping(new GetMappingRequest(indexName));
            if (mappingResponse.IsValid)
            {
                var mapping = mappingResponse.Indices[indexName].Mappings;
                Console.WriteLine("------------------- Mappings -------------------\n");
                foreach (var keyValuePair in mapping.Properties)
                {
                    Console.WriteLine($"{keyValuePair.Key}: {keyValuePair.Value}\t");
                }
            }
            else
            {
                Console.WriteLine("Failed to get mapping: " + mappingResponse.ServerError.Error);
            }
        }

        #region NOT Used Functions
        //public static void EsLogRequest ( ElasticsearchConfig config, IApiCallDetails details )
        //{
        //    if (config.Logging != null)
        //    {
        //        var logger = CreateLogger(config);
        //        if (logger != null)
        //        {
        //            if (config.Logging.LogStatusCodes)
        //                logger.LogDebug($"Request: [{details.HttpMethod}] {details.Uri}, StatusCode: {details.HttpStatusCode}");
        //            if (config.Logging.LogQueries && details?.RequestBodyInBytes != null)
        //            {
        //                string message = Encoding.UTF8.GetString(details.RequestBodyInBytes);
        //                logger.LogDebug(string.Format("\n {0}", JsonConvert.SerializeObject(JsonConvert.DeserializeObject(message), Formatting.Indented)));
        //            }
        //        }
        //    }
        //}

        //private static ILogger CreateLogger ( ElasticsearchConfig config )
        //{
        //    try
        //    {
        //        if (indexLoggers.ContainsKey(config.Node.IndexName))
        //            return indexLoggers[config.Node.IndexName];
        //        else
        //        {

        //            var logFullPath = Path.Combine(config.Logging.BaseDirectory, $"{DateTime.Now:yyyyMMdd}-{config.Node.IndexName}-es-log.log");

        //            Logger serilog = new LoggerConfiguration()
        //                .WriteTo.Console()
        //                .WriteTo.File(logFullPath, Serilog.Events.LogEventLevel.Verbose, rollingInterval: RollingInterval.Day)
        //                .MinimumLevel.Verbose()
        //                .CreateLogger();

        //            var loggerFactory = new LoggerFactory()
        //                .AddSerilog(serilog);

        //            ILogger logger = loggerFactory.CreateLogger(nameof(ClientBuilder));
        //            var result = indexLoggers.TryAdd(config.Node.IndexName, logger);
        //            int retryTimes = 3;

        //            while (!result && retryTimes >= 1)
        //            {
        //                result = indexLoggers.TryAdd(config.Node.IndexName, logger);
        //                retryTimes--;
        //            }
        //            return logger;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //} 
        #endregion

    }
}
