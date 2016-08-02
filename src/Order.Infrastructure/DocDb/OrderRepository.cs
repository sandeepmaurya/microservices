using System;
using System.Net;
using System.Threading.Tasks;
using Common.Configuration;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json.Linq;
using Order.Domain;

namespace Order.Infrastructure.DocDb
{
    public class OrderRepository : IOrderRepository
    {
        private string collectionName;
        private string databaseName;
        private string endpointUri;
        private string primaryKey;

        public OrderRepository(IConfiguration config)
        {
            this.endpointUri = config.GetValue("DocDbEndpointUri");
            this.primaryKey = config.GetValue("DocDbPrimaryKey");
            this.databaseName = config.GetValue("DocDbDatabaseName");
            this.collectionName = config.GetValue("DocDbCollectionName");
        }

        public async Task Add(Domain.Order order)
        {
            var document = order.ToDocument();
            document["id"] = order.Id;
            using (DocumentClient client = new DocumentClient(new Uri(this.endpointUri), this.primaryKey))
            {
                await client.CreateDocumentAsync(
                    UriFactory.CreateDocumentCollectionUri(this.databaseName, this.collectionName),
                    document);
            }
        }

        public async Task<Domain.Order> GetById(string orderId)
        {
            using (DocumentClient client = new DocumentClient(new Uri(this.endpointUri), this.primaryKey))
            {
                var response = await client.ReadDocumentAsync(
                    UriFactory.CreateDocumentUri(this.databaseName, this.collectionName, orderId));
                JObject doc = JObject.Parse(response.Resource.ToString());
                return OrderExtensions.FromDocument(doc);
            }
        }

        public async Task Update(Domain.Order order)
        {
            using (DocumentClient client = new DocumentClient(new Uri(this.endpointUri), this.primaryKey))
            {
                var document = order.ToDocument();
                document["id"] = order.Id;
                var response = await client.ReplaceDocumentAsync(
                    UriFactory.CreateDocumentUri(this.databaseName, this.collectionName, order.Id),
                    document);
            }
        }

        public static void Initialize(IConfiguration config)
        {
            string endpointUri = config.GetValue("DocDbEndpointUri");
            string primaryKey = config.GetValue("DocDbPrimaryKey");
            string databaseName = config.GetValue("DocDbDatabaseName");
            string collectionName = config.GetValue("DocDbCollectionName");
            CreateDatabaseIfNotExists(endpointUri, primaryKey, databaseName).Wait();
            CreateCollectionIfNotExists(endpointUri, primaryKey, databaseName, collectionName).Wait();
        }

        private static async Task CreateCollectionIfNotExists(string endpointUri, string primaryKey, string databaseName, string collectionName)
        {
            using (DocumentClient client = new DocumentClient(new Uri(endpointUri), primaryKey))
            {
                try
                {
                    await client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName));
                }
                catch (DocumentClientException de)
                {
                    // If the document collection does not exist, create a new collection
                    if (de.StatusCode == HttpStatusCode.NotFound)
                    {
                        DocumentCollection collectionInfo = new DocumentCollection();
                        collectionInfo.Id = collectionName;

                        // Configure collections for maximum query flexibility including string range queries.
                        collectionInfo.IndexingPolicy = new IndexingPolicy(new RangeIndex(DataType.String) { Precision = -1 });

                        // Here we create a collection with 400 RU/s.
                        await client.CreateDocumentCollectionAsync(
                            UriFactory.CreateDatabaseUri(databaseName),
                            collectionInfo,
                            new RequestOptions { OfferThroughput = 400 });
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        private static async Task CreateDatabaseIfNotExists(string endpointUri, string primaryKey, string databaseName)
        {
            using (DocumentClient client = new DocumentClient(new Uri(endpointUri), primaryKey))
            {
                try
                {
                    await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(databaseName));
                }
                catch (DocumentClientException de)
                {
                    // If the database does not exist, create a new database
                    if (de.StatusCode == HttpStatusCode.NotFound)
                    {
                        await client.CreateDatabaseAsync(new Database { Id = databaseName });
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }
    }
}
