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
    public class Rest
    {
        public readonly HttpClient client;

        public Rest(bool validateSSLCerts = true)
        {
            var handler = new WebRequestHandler();
            if(!validateSSLCerts) handler.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            client = new HttpClient(handler);
        }

        public async Task<string> PostAsync(string uri, string json)
        {
            StringContent requestContent = new StringContent(json, Encoding.UTF8, "application/json");
            return await PostAsync(uri, requestContent);
        }

        public async Task<string> PostAsync(string uri, HttpContent data)
        {
            HttpContent responseContent;
            HttpStatusCode responseCode;

            // Get the response.
            try
            {
                HttpResponseMessage response = await client.PostAsync(uri, data);

                // Get the response content.
                responseContent = response.Content;
                responseCode = response.StatusCode;
            }
            catch (ArgumentNullException ex)
            {
                throw;
            }
            catch (HttpRequestException ex)
            {
                throw;
            }

            if ((int)responseCode < 200 || (int)responseCode >= 300)
            {
                using (var reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
                {
                    var response = await reader.ReadToEndAsync();
                    // Write the output.
                    throw new RestException("Bad HTTP responseCode: " + (int)responseCode, (int)responseCode, response);
                }
            }

            // Get the stream of the content.
            using (var reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
            {
                var response = await reader.ReadToEndAsync();
                // Write the output.
                return response;
            }
        }

        public async Task<string> GetAsync(string uri)
        {

            HttpContent responseContent;
            HttpStatusCode responseCode;

            // Get the response.
            try
            {
                HttpResponseMessage response = await client.GetAsync(uri);

                // Get the response content.
                responseContent = response.Content;
                responseCode = response.StatusCode;
            }
            catch (ArgumentNullException ex)
            {
                throw;
            }
            catch (HttpRequestException ex)
            {
                throw;
            }

            if ((int)responseCode < 200 || (int)responseCode >= 300)
            {
                using (var reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
                {
                    var response = await reader.ReadToEndAsync();
                    // Write the output.
                    throw new RestException("Bad HTTP responseCode: " + (int)responseCode, (int)responseCode, response);
                }
            }

            // Get the stream of the content.
            using (var reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
            {
                var response = await reader.ReadToEndAsync();
                // Write the output.
                return response;
            }
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

