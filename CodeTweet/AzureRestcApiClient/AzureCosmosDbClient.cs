using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AzureRestcApiClient
{
    public class WebConsomsDbClient : IConsomsDbClient

    {

        public string MasterKey { get; }

        public Uri BaseUri { get; }



        private HttpClient _client = new HttpClient();



        private const string ApiDate = "2015-08-06";

        private const string KeyType = "master";

        private const string TokenVersion = "1.0";



        private const string DatabaseResourceTypeId = "dbs";

        private const string CollectionResourceTypeId = "colls";

        private const string DocumentsResourceTypeId = "docs";



        public WebConsomsDbClient(string masterKey, Uri baseUri)

        {

            MasterKey = masterKey;

            BaseUri = baseUri;

        }



        private string GetAuthorizationDateTime => DateTime.UtcNow.ToString("r");



        public async Task<string> GetAllDatabases()

        {

            return await MakeRestCall(method: HttpMethod.Get,

                baseUri: BaseUri,

                resourceId: "",

                resourceType: DatabaseResourceTypeId,

                resourceLink: DatabaseResourceTypeId,

                key: MasterKey,

                keyType: KeyType,

                tokenVersion: TokenVersion,

                utcDate: GetAuthorizationDateTime,

                query: null);



        }



        public async Task<string> GetDatabase(string databaseId)

        {

            return await MakeRestCall(method: HttpMethod.Get,

                baseUri: BaseUri,

                resourceId: $"{DatabaseResourceTypeId}/{databaseId}",

                resourceType: DatabaseResourceTypeId,

                resourceLink: $"{DatabaseResourceTypeId}/{databaseId}",

                key: MasterKey,

                keyType: KeyType,

                tokenVersion: TokenVersion,

                utcDate: GetAuthorizationDateTime,

                query: null);

        }



        public async Task<string> GetAllCollections(string databaseId)

        {

            return await MakeRestCall(method: HttpMethod.Get,

                baseUri: BaseUri,

                resourceId: $"{DatabaseResourceTypeId}/{databaseId}",

                resourceType: CollectionResourceTypeId,

                resourceLink: $"{DatabaseResourceTypeId}/{databaseId}/{CollectionResourceTypeId}",

                key: MasterKey,

                keyType: KeyType,

                tokenVersion: TokenVersion,

                utcDate: GetAuthorizationDateTime,

                query: null);

        }



        public async Task<string> GetCollection(string databaseId, string collectionId)

        {

            return await MakeRestCall(method: HttpMethod.Get,

                baseUri: BaseUri,

                resourceId: $"{DatabaseResourceTypeId}/{databaseId}/{CollectionResourceTypeId}/{collectionId}",

                resourceType: CollectionResourceTypeId,

                resourceLink: $"{DatabaseResourceTypeId}/{databaseId}/{CollectionResourceTypeId}/{collectionId}",

                key: MasterKey,

                keyType: KeyType,

                tokenVersion: TokenVersion,

                utcDate: GetAuthorizationDateTime,

                query: null);

        }



        public async Task<string> GetAllDocuments(string databaseId, string collectionId)

        {

            return await MakeRestCall(method: HttpMethod.Get,

                baseUri: BaseUri,

                resourceId: $"{DatabaseResourceTypeId}/{databaseId}/{CollectionResourceTypeId}/{collectionId}",

                resourceType: DocumentsResourceTypeId,

                resourceLink: $"{DatabaseResourceTypeId}/{databaseId}/{CollectionResourceTypeId}/{collectionId}/{DocumentsResourceTypeId}",

                key: MasterKey,

                keyType: KeyType,

                tokenVersion: TokenVersion,

                utcDate: GetAuthorizationDateTime,

                query: null);

        }



        public async Task<string> GetDocument(string databaseId, string collectionId, string documentId)

        {

            return await MakeRestCall(method: HttpMethod.Get,

                baseUri: BaseUri,

                resourceId: $"{DatabaseResourceTypeId}/{databaseId}/{CollectionResourceTypeId}/{collectionId}/{DocumentsResourceTypeId}/{documentId}",

                resourceType: DocumentsResourceTypeId,

                resourceLink: $"{DatabaseResourceTypeId}/{databaseId}/{CollectionResourceTypeId}/{collectionId}/{DocumentsResourceTypeId}/{documentId}",

                key: MasterKey,

                keyType: KeyType,

                tokenVersion: TokenVersion,

                utcDate: GetAuthorizationDateTime,

                query: null);

        }



        public async Task<string> ExecuteQuery(string databaseId, string collectionId, string query, params KeyValuePair<string, string>[] parameters)

        {

            return await MakeRestCall(method: HttpMethod.Post,

                baseUri: BaseUri,

                resourceId: $"{DatabaseResourceTypeId}/{databaseId}/{CollectionResourceTypeId}/{collectionId}",

                resourceType: DocumentsResourceTypeId,

                resourceLink: $"{DatabaseResourceTypeId}/{databaseId}/{CollectionResourceTypeId}/{collectionId}/{DocumentsResourceTypeId}",

                key: MasterKey,

                keyType: KeyType,

                tokenVersion: TokenVersion,

                utcDate: GetAuthorizationDateTime,

                query: query,

                parameters: parameters);

        }



        private async Task<string> MakeRestCall(HttpMethod method, Uri baseUri, string resourceId, string resourceType, string resourceLink, string key, string keyType, string tokenVersion, string utcDate, string query = null, KeyValuePair<string, string>[] parameters = null)

        {

            var uri = new Uri(baseUri, resourceLink);



            var authenticationHeader = GenerateMasterKeyAuthorizationSignature(method.ToString(), resourceId, resourceType, key, keyType, tokenVersion, utcDate);



            using (var request = new HttpRequestMessage(method, uri))

            {

                SetHeaders(_client, request, authenticationHeader, ApiDate, utcDate, null != query);



                HttpResponseMessage response;



                if (null != query)

                {

                    var args = new List<KeyValuePair<string, string>>(parameters) { new KeyValuePair<string, string>("query", query) };



                    request.Content = new StringContent(JsonConvert.SerializeObject(new SqlQueryToken(query, parameters)));

                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/query+json");

                    request.Content.Headers.ContentType.CharSet = "";



                    response = await _client.SendAsync(request);



                }

                else

                {

                    response = await _client.SendAsync(request);

                }



                return await response.Content.ReadAsStringAsync();

            }

        }



        private void SetHeaders(HttpClient client, HttpRequestMessage request, string authenticationHeader, string apiVersionDate, string utcDate, bool isQuery)

        {

            SetHeader(client, request, "x-ms-date", utcDate);



            SetHeader(client, request, "authorization", authenticationHeader);



            SetHeader(client, request, "x-ms-version", apiVersionDate);



            if (isQuery)

            {

                SetHeader(client, request, "x-ms-documentdb-isquery", "True");

            }

        }



        private void SetHeader(HttpClient client, HttpRequestMessage request, string headerName, string headerValue, bool defaultOnly = false)

        {

            client.DefaultRequestHeaders.Remove(headerName);

            client.DefaultRequestHeaders.Add(headerName, headerValue);



            if (!defaultOnly)

            {

                request.Headers.Remove(headerName);

                request.Headers.Add(headerName, headerValue);

            }



        }



        private string GenerateMasterKeyAuthorizationSignature(string verb, string resourceId, string resourceType, string key, string keyType, string tokenVersion, string utcDate)

        {

            using (var hmacSha256 = new HMACSHA256(Convert.FromBase64String(key)))

            {

                var payLoad = $"{verb.ToLowerInvariant()}\n{resourceType.ToLowerInvariant()}\n{resourceId}\n{utcDate.ToLowerInvariant()}\n\n";//.ToLowerInvariant();

                var signature = Convert.ToBase64String(hmacSha256.ComputeHash(Encoding.UTF8.GetBytes(payLoad)));



                return Uri.EscapeDataString($"type={keyType}&ver={tokenVersion}&sig={signature}");

            }

        }



        public void Dispose()

        {

            Dispose(true);

        }



        protected virtual void Dispose(bool disposing)

        {

            if (disposing)

            {

                _client?.Dispose();

                _client = null;

            }

        }

    }
}
