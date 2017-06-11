using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace AzureRestcApiClient
{
    public class QueryResult<T>

    {

        [JsonProperty(PropertyName = "_rid")]

        public string ResourceId { get; set; }

        public T[] Documents { get; set; }

    }
}
