using Demo.Configuration;
using Demo.Enums;
using Demo.Models;
using iucon.Onix.ElasticSearch.Builders;
using iucon.Onix.ElasticSearch.Repositories;

var esconfig = new ElasticsearchConfig();
//var demoIndex =
var connectionString = ClientBuilder.BuildConnectionSettings<demoindex>(esconfig);
var client = ClientBuilder.CreateIndexIfNotExistsAsync<demoindex>(nameof(demoindex), connectionString);
var indexRepository = new IndexRepository<demoindex>(client);
#region INDEXING
//var firstDoc = new demoindex { ProductReference = "123", Price = 300, ProductName = "Test Product" };
//var response = await indexRepository.IndexDocument(firstDoc);
//if (response.IsValid)
//{
//    Console.WriteLine($"{response.Result} \n -------------------- \n Document \n" +
//        $"{firstDoc.ProductReference} \n" +
//        $"{firstDoc.ProductName} \n" +
//        $"{firstDoc.Price}\n -------------------------------------------------------");
//}

//demoindex[ ] someDocs = new demoindex[ ] {
//new demoindex { ProductReference = "124", Price = 300, ProductName = "Test2 Product" },
//new demoindex { ProductReference = "125", Price = 400, ProductName = "Test3 Product21" },
//new demoindex { ProductReference = "126", Price = 4008, ProductName = "Test243 Product" },
//new demoindex { ProductReference = "127", Price = 4006, ProductName = "Tes221t3 Product" },
//new demoindex { ProductReference = "128", Price = 40052, ProductName = "Test324 Product" },
//new demoindex { ProductReference = "129", Price = 4060, ProductName = "Tes224t3 Product" },
//new demoindex { ProductReference = "130", Price = 4400, ProductName = "Test3 2424Product" },
//new demoindex { ProductReference = "131", Price = 4050, ProductName = "Test3 Prod242uct" },
//new demoindex { ProductReference = "132", Price = 4600, ProductName = "Test3 Product12245" },
//new demoindex { ProductReference = "133", Price = 4060, ProductName = "Test3 Prod21201uct" },
//new demoindex { ProductReference = "134", Price = 4050, ProductName = "Test3 Prod21658uct" },
//new demoindex { ProductReference = "135", Price = 4500, ProductName = "Test3 Prod242uct" },
//new demoindex { ProductReference = "136", Price = 4050, ProductName = "Test3 Pr21211oduct" },
//new demoindex { ProductReference = "137", Price = 4600, ProductName = "Test3 Produc24121t" },
//new demoindex { ProductReference = "138", Price = 40690, ProductName = "Test5313 Pro24duct" },
//new demoindex { ProductReference = "139", Price = 406960, ProductName = "Test3 Pr4210oduct" },
//new demoindex { ProductReference = "140", Price = 405250, ProductName = "Test3 42Product" },
//new demoindex { ProductReference = "141", Price = 40250, ProductName = "Test32535 Product" },
//new demoindex { ProductReference = "142", Price = 40620, ProductName = "Test321 Product" },
//new demoindex { ProductReference = "143", Price = 40630, ProductName = "Test3 Product124" },
//new demoindex { ProductReference = "145", Price = 40620, ProductName = "Test3 Produc56542t" },
//new demoindex { ProductReference = "146", Price = 403220, ProductName = "Test3 Produc563t" },
//new demoindex { ProductReference = "147", Price = 40330, ProductName = "Test3 Produ511ct" },
//new demoindex { ProductReference = "148", Price = 4003, ProductName = "Test3 Prod51uct" },
//new demoindex { ProductReference = "149", Price = 40036, ProductName = "Test3 P35roduct" },
//new demoindex { ProductReference = "150", Price = 400354, ProductName = "Test3 Produc24t" },
//new demoindex { ProductReference = "151", Price = 4600, ProductName = "Test3 Produc24t" },
//new demoindex { ProductReference = "152", Price = 40630, ProductName = "Test3 Produc215t" },
//new demoindex { ProductReference = "153", Price = 4003, ProductName = "Test3 Produc153t" },
//new demoindex { ProductReference = "154", Price = 40360, ProductName = "Test3 Produ15ct" },
//new demoindex { ProductReference = "155", Price = 40036, ProductName = "Test3 Produc153t" },
//new demoindex { ProductReference = "156", Price = 54400, ProductName = "Test3 Pro5132duct" },
//new demoindex { ProductReference = "157", Price = 40450, ProductName = "Test3 P45roduct" },
//new demoindex { ProductReference = "158", Price = 408960, ProductName = "Tes6456t3 Product" },
//new demoindex { ProductReference = "159", Price = 85400, ProductName = "Test3548 Product" },
//new demoindex { ProductReference = "160", Price = 65400, ProductName = "Test3515 Product" },
//new demoindex { ProductReference = "161", Price = 45400, ProductName = "Test3564 Product" },
//new demoindex { ProductReference = "162", Price = 458400, ProductName = "Test323 Product" },
//new demoindex { ProductReference = "163", Price = 63400, ProductName = "Test3452 Product" },
//new demoindex { ProductReference = "164", Price = 458400, ProductName = "Test369 Product" },
//new demoindex { ProductReference = "165", Price = 456400, ProductName = "Test345 Product" },
//new demoindex { ProductReference = "166", Price = 412400, ProductName = "Test345 Product" },
//new demoindex { ProductReference = "167", Price = 152400, ProductName = "Test356 Product" },
//new demoindex { ProductReference = "168", Price = 489400, ProductName = "Test35 Product" },
//new demoindex { ProductReference = "1266", Price = 4556500, ProductName = "Test4 Product" }
//};

//var responseBulkCreat = await indexRepository.BulkCreate(DocumentArray: someDocs, indexName: nameof(demoindex));
//if (!responseBulkCreat.Errors)
//{
//    foreach (var doc in someDocs)
//    {
//        Console.WriteLine($"Key: {nameof(doc.ProductReference)} --- Value: {doc.ProductReference}");
//        Console.WriteLine($"Key: {nameof(doc.ProductName)} --- Value: {doc.ProductName}");
//        Console.WriteLine($"Key: {nameof(doc.Price)} --- Value: {doc.Price} \n \t---------------------------------------------");
//    }
//} 
#endregion

var responseMatchAll = await indexRepository.MatchAllQuery();
if (responseMatchAll.IsValid)
{
    Console.WriteLine("\n \n------------------------------------- Match All Query ------------------------------------- ");
    foreach (var doc in responseMatchAll.Hits)
    {
        var docSingle = doc.Source;

        Console.WriteLine($"Key: {nameof(docSingle.ProductReference)} --- Value: {docSingle.ProductReference}");
        Console.WriteLine($"Key: {nameof(docSingle.ProductName)} --- Value: {docSingle.ProductName}");
        Console.WriteLine($"Key: {nameof(docSingle.Price)} --- Value: {docSingle.Price} \n --------------------------------------------------------------------");
    }
}

var searchResponse = await indexRepository.BoolQuery("168", "Test35", 489400);
if (searchResponse.IsValid)
{
    Console.WriteLine("\n \n------------------------------------- Bool Query ------------------------------------- ");
    foreach (var doc in searchResponse.Hits)
    {
        var docSingle = doc.Source;

        Console.WriteLine($"Key: {nameof(docSingle.ProductReference)} --- Value: {docSingle.ProductReference}");
        Console.WriteLine($"Key: {nameof(docSingle.ProductName)} --- Value: {docSingle.ProductName}");
        Console.WriteLine($"Key: {nameof(docSingle.Price)} --- Value: {docSingle.Price} \n --------------------------------------------------------------------");
    }
}

var sumResponse = await indexRepository.PreformSumAggregation(nameof(DemoIndexFieldsName.price));
if (sumResponse.IsValid)
{
    Console.WriteLine("\n \n------------------------------------- SUM Query ------------------------------------- ");
    Console.WriteLine(sumResponse.Aggregations.Sum("total_price").Value);
}
var filtersResponse = await indexRepository.PerformFilterAggregation(nameof(DemoIndexFieldsName.productReference));
if (filtersResponse.IsValid)
{
    Console.WriteLine("\n \n-------------------------------------[ Product refrances starts with 14.... ]------------------------------------- ");
    var bucketAggregation = filtersResponse.Aggregations.Filters("filters_buckets");
    var namedBucket = bucketAggregation.NamedBucket("Product refrances starts with 14....");
    Console.WriteLine($"Documents count: {namedBucket.DocCount}");
    var topHits = namedBucket.TopHits("my_docs");
    foreach (var docSingle in topHits.Documents<demoindex>())
    {
        Console.WriteLine($"Key: {nameof(docSingle.ProductReference)} --- Value: {docSingle.ProductReference}");
        Console.WriteLine($"Key: {nameof(docSingle.ProductName)} --- Value: {docSingle.ProductName}");
        Console.WriteLine($"Key: {nameof(docSingle.Price)} --- Value: {docSingle.Price} \n --------------------------------------------------------------------");
    }
}

