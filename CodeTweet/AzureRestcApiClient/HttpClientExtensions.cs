using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AzureRestcApiClient
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> PostWithNoCharSetAsync<T>(this HttpClient client, Uri requestUri, T value)
        {

            //return await client.PostAsync(requestUri, value, new NoCharSetJsonMediaTypeFormatter());

            //return await client.PostAsync(requestUri.ToString(), value, new NoCharSetJsonMediaTypeFormatter());

            //return await client.PostAsJsonAsync(requestUri.ToString(), value);
            
            return await client.PostAsync(requestUri.ToString(), new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/query+json"));
            
            
        }
    }
}
