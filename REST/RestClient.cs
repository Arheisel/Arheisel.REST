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
        private readonly Uri _baseURL;
        private readonly HttpClient _httpClient;

        public RestClient(string baseURL, bool validateSSLCerts = true)
        {
            if (string.IsNullOrEmpty(baseURL)) _baseURL = null;
            else _baseURL = new Uri(baseURL);

            var handler = new WebRequestHandler();
            if (!validateSSLCerts) handler.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            _httpClient = new HttpClient();
        }

        public async Task<T> GetAsync<T>(string route)
        {
            if (_baseURL == null) throw new NullReferenceException("BaseURL is not set");

            var newUri = new Uri(_baseURL, route);

            return await HandleResponse<T>(await _httpClient.GetAsync(newUri));
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
            if (_baseURL == null) throw new NullReferenceException("BaseURL is not set");
            var newUri = new Uri(_baseURL, route);

            return await HandleResponse<T>(await _httpClient.PostAsync(newUri, payload));
        }

        private async Task<T> HandleResponse<T>(HttpResponseMessage response)
        {
            var responseCode = (int)response.StatusCode;
            var responseText = responseCode == 204 ? null : await response.Content?.ReadAsStringAsync();

            if (responseCode < 200 || responseCode >= 300) 
                throw new RestException("Bad HTTP Code: " + responseCode, responseCode, responseText);

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

