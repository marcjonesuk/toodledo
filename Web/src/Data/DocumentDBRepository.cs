using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace Data
{
    public interface IRepository
    {
        void Initialize();
        Task<T> GetAsync<T>(string id);
        Task AddAsync(object o);
        Task DeleteAsync(string id);
        Task UpdateAsync(string id, object o);
    }

    public class AzureRepository : IRepository
    {
        private static readonly string Endpoint = "https://toodledo-dev.documents.azure.com:443/";
        private static readonly string Key = "";
        private static readonly string DatabaseId = "dev";
        private static readonly string CollectionId = "content";
        private static DocumentClient client;

        public void Initialize()
        {
            client = new DocumentClient(new Uri(Endpoint), Key);
        }

        public Task DeleteAsync(string id)
        {
            return client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id));
        }

        public async Task<T> GetAsync<T>(string id)
        {
            try
            {
                Document document = await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id));
                return (T)(dynamic)document;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return default(T);
                }
                else
                {
                    throw;
                }
            }
        }
        
        public Task UpdateAsync(string id, object o)
        {
            return client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id), o);
        }

        public Task AddAsync(object o)
        {
            return client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), o);
        }
    }

    public class LocalRepository : IRepository
    {
        private static DocumentClient client;
        private Dictionary<string, object> _data = new Dictionary<string, object>();

        public Task DeleteAsync(string id)
        {
            _data.Remove(id);
            return Task.CompletedTask;
        }

        public Task<T> GetAsync<T>(string id)
        {
            return Task.FromResult<T>((T)_data[id]);
        }

        public void Initialize()
        {
        }

        public Task AddAsync(object o)
        {
            dynamic d = o;
            _data[d.Id] = o;
            return Task.CompletedTask;
        }

        public Task UpdateAsync(string id, object o)
        {
            dynamic d = o;
            d.Id = o;
            if (!_data.ContainsKey(d.Id))
                throw new Exception("argh");
            return AddAsync(o);
        }
    }

    public static class DocumentDBRepository<T> where T : class
    {
        private static readonly string Endpoint = "https://toodledo-dev.documents.azure.com:443/";
        private static readonly string Key = "xhOojY70whyrSfkfB54mnzO28oxArobVMh3a9YVPqNEys4lVoEffRFcGaEZ8hqVLefIcrn9UohbUCYV8HVhXxw==";
        private static readonly string DatabaseId = "dev";
        private static readonly string CollectionId = "content";

        private static DocumentClient client;

        public static async Task<T> GetItemAsync(string id)
        {
            try
            {
                Document document = await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id));
                return (T)(dynamic)document;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public static async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate)
        {
            IDocumentQuery<T> query = client.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                new FeedOptions { MaxItemCount = -1 })
                .Where(predicate)
                .AsDocumentQuery();

            List<T> results = new List<T>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }

            return results;
        }

        public static async Task<Document> CreateItemAsync(T item)
        {
            return await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), item);
        }

        public static async Task<Document> UpdateItemAsync(string id, T item)
        {
            return await client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id), item);
        }

        public static async Task DeleteItemAsync(string id)
        {
            await client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id));
        }

        public static void Initialize()
        {
            client = new DocumentClient(new Uri(Endpoint), Key);
            CreateDatabaseIfNotExistsAsync().Wait();
            CreateCollectionIfNotExistsAsync().Wait();
        }

        private static async Task CreateDatabaseIfNotExistsAsync()
        {
            try
            {
                await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(DatabaseId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDatabaseAsync(new Database { Id = DatabaseId });
                }
                else
                {
                    throw;
                }
            }
        }

        private static async Task CreateCollectionIfNotExistsAsync()
        {
            try
            {
                await client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(DatabaseId),
                        new DocumentCollection { Id = CollectionId },
                        new RequestOptions { OfferThroughput = 1000 });
                }
                else
                {
                    throw;
                }
            }
        }
    }
}