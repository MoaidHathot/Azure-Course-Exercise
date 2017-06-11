using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AzureRestcApiClient
{
    public interface IConsomsDbClient : IDisposable

    {

        Task<string> GetAllDatabases();

        Task<string> GetDatabase(string databaseId);

        Task<string> GetAllCollections(string databaseId);

        Task<string> GetCollection(string databaseId, string collectionId);

        Task<string> GetAllDocuments(string databaseId, string collectionId);

        Task<string> GetDocument(string databaseId, string collectionId, string documentId);

        Task<string> ExecuteQuery(string databaseId, string collectionId, string query, KeyValuePair<string, string>[] parameters);

    }
}
