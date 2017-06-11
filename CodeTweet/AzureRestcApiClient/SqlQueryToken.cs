using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzureRestcApiClient
{
    public class SqlQueryToken
    {
        public string query { get; set; }

        public object[] parameters { get; set; }

        public SqlQueryToken(string query, KeyValuePair<string, string>[] parameters)
        {
            this.query = query;

            this.parameters = parameters.Select(pair => new { name = pair.Key, value = pair.Value }).ToArray();
        }
    }
}
