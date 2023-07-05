//using iucon.Onix.Domain.Dtos;
//using iucon.Onix.ElasticSearch.CustomVisitors;
//using Nest;
//using Newtonsoft.Json;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace iucon.Onix.ElasticSearch.Extensions
//{
//    public static class ElasticClientExtensions
//    {
//        public static Task<CreateIndexResponse> CreateAsync<TDocument>(this ElasticClient client, string indexName, string? indexSettings = null) where TDocument : class
//        {
//            Models.IndexSettings? _settings = null;
//            if (indexSettings != null)
//                _settings = JsonConvert.DeserializeObject<Models.IndexSettings>(indexSettings);

//            return client.Indices.CreateAsync(indexName,
//                   index => index.InitializeUsing(new IndexState() { Settings = _settings?.ToNestSettings() ?? GetDefaultSettings() })
//                                .Map<TDocument>(x => x.AutoMap(new CustomPropretyVisitor())));
//        }

//        public static Task<PutMappingResponse> UpdateIndexMapping<TDocument>(this ElasticClient client, string indexName) where TDocument : class
//            => client.Indices.PutMappingAsync<TDocument>(m => m.AutoMap(new CustomPropretyVisitor()).Index(indexName));

//        public static async Task<bool> ExistsAsync(this ElasticClient client, string indexName)
//            => (await client.Indices.ExistsAsync(indexName)).Exists;

//        private static IndexSettings GetDefaultSettings()
//        {
//            var _settings = new IndexSettings()
//            {
//                NumberOfReplicas = 3,
//                NumberOfShards = 3,
//            };
//            _settings.Add("index.mapping.total_fields.limit", 6000);

//            _settings.Analysis = new Analysis();
//            AddNormalizers((Analysis)_settings.Analysis, new Models.IndexSettings()
//            {
//                Normalizers = new List<Analyzer>()
//                {
//                   new Analyzer()
//                   {
//                       Name="keyword_normalizer",
//                       Filter = new List<string>()
//                       {
//                           "lowercase"
//                       }
//                   }
//                }
//            });
//            return _settings;
//        }

//        private static IndexSettings? ToNestSettings(this Models.IndexSettings? settings)
//        {
//            if (settings == null)
//                return null;

//            IndexSettings nestSettings = new IndexSettings();
//            Analysis analysis = new Analysis();
//            nestSettings.Analysis = analysis;

//            AddAnalyzers(analysis, settings);
//            AddNormalizers(analysis, settings);

//            nestSettings.NumberOfReplicas = settings.NumberOfReplicas;
//            nestSettings.NumberOfShards = settings.NumberOfShards;

//            if (settings?.Mapping?.TotalFields?.Limit != null)
//                nestSettings.Add("index.mapping.total_fields.limit", settings.Mapping.TotalFields.Limit);

//            return nestSettings;
//        }

//        private static void AddAnalyzers(Analysis analysis, Models.IndexSettings settings)
//        {
//            if (settings.Analyzers == null || !settings.Analyzers.Any())
//                return;

//            analysis.Analyzers = new Analyzers();

//            foreach (var analyzer in settings.Analyzers)
//            {
//                var customAnalyzer = new CustomAnalyzer
//                {
//                    CharFilter = analyzer.CharFilter,
//                    Tokenizer = analyzer.Tokenizer,
//                    Filter = analyzer.Filter
//                };

//                analysis.Analyzers.Add(analyzer.Name, customAnalyzer);
//            }
//        }

//        private static void AddNormalizers(Analysis analysis, Models.IndexSettings settings)
//        {
//            if (settings.Normalizers == null || !settings.Normalizers.Any())
//                return;

//            analysis.Normalizers = new Normalizers();

//            foreach (var normalizer in settings.Normalizers)
//            {
//                var customAnalyzer = new CustomNormalizer
//                {
//                    CharFilter = normalizer.CharFilter,
//                    Filter = normalizer.Filter
//                };

//                analysis.Normalizers.Add(normalizer.Name, customAnalyzer);
//            }
//        }
//    }
//}
