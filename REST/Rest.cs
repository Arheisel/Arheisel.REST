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
        public string Token { get; private set; } = string.Empty;
        public string TokenType { get; private set; } = string.Empty;

        public Rest()
        {
            client = new HttpClient();
            //specify to use TLS 1.2 as default connection
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
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
                HttpResponseMessage response = await client.PostAsync(
                    uri,
                    data);

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

        public static string JsonFromTemplate(string name, Dictionary<string, string> args)
        {
            /* BEWARE, HIGH SECURITY RISK:
             * NEVER FEED USER INPUT INTO PATH.COMBINE
             * As per Microsoft Docs: if an argument other than the first contains a rooted path, any previous path components are ignored, 
             * and the returned string begins with that rooted path component.
            */
            var fname = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"templates\" + name + ".txt");

            if (!File.Exists(fname)) throw new Exception("File " + fname + "not found.");

            var json = File.ReadAllText(fname);

            foreach (KeyValuePair<string, string> arg in args)
            {
                json = json.Replace("{{" + arg.Key + "}}", arg.Value);
            }

            return json;
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

