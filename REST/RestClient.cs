using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Arheisel.REST
{
    public class RestClient
    {
        private readonly HttpClient _httpClient;

        public RestClient(string baseURL, bool validateSSLCerts = true)
        {
            if (string.IsNullOrEmpty(baseURL)) throw new ArgumentNullException(nameof(baseURL));

            var handler = new HttpClientHandler();
            if (!validateSSLCerts) handler.ServerCertificateCustomValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(baseURL)
            };
        }

        public async Task<T> GetAsync<T>(string route)
        {
            return await HandleResponse<T>(await _httpClient.GetAsync(new Uri(route, UriKind.Relative)));
        }

        public async Task<T> PostAsync<T>(string route, object payload)
        {
            StringContent requestContent = null;

            if (payload != null)
            {
                var json = JsonConvert.SerializeObject(payload);
                requestContent = new StringContent(json, Encoding.UTF8, "application/json");
            }

            return await PostAsync<T>(route, requestContent);
        }

        public async Task<T> PostAsync<T>(string route, HttpContent payload)
        {
            return await HandleResponse<T>(await _httpClient.PostAsync(new Uri(route, UriKind.Relative), payload));
        }

        private async Task<T> HandleResponse<T>(HttpResponseMessage response)
        {
            var responseCode = (int)response.StatusCode;
            var responseText = responseCode == 204 ? null : await response.Content?.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new RestException($"Error received ({responseCode}) from {response.RequestMessage.RequestUri}", responseCode, responseText);

            if (string.IsNullOrEmpty(responseText)) return default;

            return JsonConvert.DeserializeObject<T>(await response.Content?.ReadAsStringAsync());
        }
    }

    public class RestException : Exception
    {
        public int ResponseCode { get; private set; }
        public string Response { get; private set; }
        public RestException(string message, int responseCode, string response) : base(message)
        {
            ResponseCode = responseCode;
            Response = response;
        }
    }
}

