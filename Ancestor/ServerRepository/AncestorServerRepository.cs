using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Ancestor.Extensions;

namespace Ancestor.ServerRepository
{
    public abstract class AncestorServerRepository
    {
        public abstract Dictionary<string, string> ProvideHeaders(string endpoint, string body);
        public abstract T ParseResponse<T>(string response);

        private T Execute<T>(Func<HttpClient, string, StringContent, Task<HttpResponseMessage>> method, string schema, string serverAddress, int port, string endpoint, object input = null)
        {
            var uri = new UriBuilder(schema, serverAddress, port, endpoint);

            var path = uri.Uri.ToString();

            using var httpClient = new HttpClient();

            string body = input?.ToJson() ?? string.Empty;

            var headers= ProvideHeaders(endpoint, body);

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            StringContent content = new StringContent(body, Encoding.UTF8, "application/json");

            HttpResponseMessage response = method(httpClient, path, content).Result;

            string responseString = response.Content.ReadAsStringAsync().Result;

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(response.RequestMessage.RequestUri + Environment.NewLine + responseString, exception);
            }

            return ParseResponse<T>(responseString);
        }

        public T Post<T>(string schema, string serverAddress, int port, string endpoint, object input = null)
        {
            return Execute<T>((
                    client, path, content) => client.PostAsync(path, content), 
                schema, serverAddress, port, endpoint, input
                );
        }

        public T Put<T>(string schema, string serverAddress, int port, string endpoint, object input = null)
        {
            return Execute<T>((
                    client, path, content) => client.PutAsync(path, content),
                schema, serverAddress, port, endpoint, input
            );
        }

        public T Get<T>(string schema, string serverAddress, int port, string endpoint, object input = null)
        {
            return Execute<T>((
                    client, path, content) => client.GetAsync(path),
                schema, serverAddress, port, endpoint, input
            );
        }
    }
}
