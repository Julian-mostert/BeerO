using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using BeerO.SpotifyApi.Models;
using Newtonsoft.Json;

namespace BeerO.SpotifyApi
{
    internal class SpotifyWebClient : IClient
    {
        public JsonSerializerSettings JsonSettings { get; set; }
        private readonly Encoding _encoding = Encoding.UTF8;
        private readonly HttpClient _client;

        private const string UnknownErrorJson = "{\"error\": { \"status\": 0, \"message\": \"BeerO.SpotifyApi - Unkown Spotify Error\" }}";

        public SpotifyWebClient(ProxyConfig proxyConfig = null)
        {
            HttpClientHandler clientHandler = CreateClientHandler(proxyConfig);
            this._client = new HttpClient(clientHandler);
        }

        public Tuple<ResponseInfo, string> Download(string url, Dictionary<string, string> headers = null)
        {
            Tuple<ResponseInfo, byte[]> raw = this.DownloadRaw(url, headers);
            return new Tuple<ResponseInfo, string>(raw.Item1, raw.Item2.Length > 0 ? this._encoding.GetString(raw.Item2) : "{}");
        }

        public async Task<Tuple<ResponseInfo, string>> DownloadAsync(string url, Dictionary<string, string> headers = null)
        {
            Tuple<ResponseInfo, byte[]> raw = await this.DownloadRawAsync(url, headers).ConfigureAwait(false);
            return new Tuple<ResponseInfo, string>(raw.Item1, raw.Item2.Length > 0 ? this._encoding.GetString(raw.Item2) : "{}");
        }

        public Tuple<ResponseInfo, byte[]> DownloadRaw(string url, Dictionary<string, string> headers = null)
        {
            if (headers != null)
            {
                this.AddHeaders(headers);
            }
            using (HttpResponseMessage response = Task.Run(() => this._client.GetAsync(url)).Result)
            {
                return new Tuple<ResponseInfo, byte[]>(new ResponseInfo
                {
                    StatusCode = response.StatusCode,
                    Headers = ConvertHeaders(response.Headers)
                }, Task.Run(() => response.Content.ReadAsByteArrayAsync()).Result);
            }
        }

        public async Task<Tuple<ResponseInfo, byte[]>> DownloadRawAsync(string url, Dictionary<string, string> headers = null)
        {
            if (headers != null)
            {
                this.AddHeaders(headers);
            }
            using (HttpResponseMessage response = await this._client.GetAsync(url).ConfigureAwait(false))
            {
                return new Tuple<ResponseInfo, byte[]>(new ResponseInfo
                {
                    StatusCode = response.StatusCode,
                    Headers = ConvertHeaders(response.Headers)
                }, await response.Content.ReadAsByteArrayAsync());
            }
        }

        public Tuple<ResponseInfo, T> DownloadJson<T>(string url, Dictionary<string, string> headers = null)
        {
            Tuple<ResponseInfo, string> response = this.Download(url, headers);
            try
            {
                return new Tuple<ResponseInfo, T>(response.Item1, JsonConvert.DeserializeObject<T>(response.Item2, this.JsonSettings));
            }
            catch (JsonException)
            {
                return new Tuple<ResponseInfo, T>(response.Item1, JsonConvert.DeserializeObject<T>(UnknownErrorJson, this.JsonSettings));
            }
        }

        public async Task<Tuple<ResponseInfo, T>> DownloadJsonAsync<T>(string url, Dictionary<string, string> headers = null)
        {
            Tuple<ResponseInfo, string> response = await this.DownloadAsync(url, headers).ConfigureAwait(false);try
            {
                return new Tuple<ResponseInfo, T>(response.Item1, JsonConvert.DeserializeObject<T>(response.Item2, this.JsonSettings));
            }
            catch (JsonException)
            {
                return new Tuple<ResponseInfo, T>(response.Item1, JsonConvert.DeserializeObject<T>(UnknownErrorJson, this.JsonSettings));
            }
        }

        public Tuple<ResponseInfo, string> Upload(string url, string body, string method, Dictionary<string, string> headers = null)
        {
            Tuple<ResponseInfo, byte[]> data = this.UploadRaw(url, body, method, headers);
            return new Tuple<ResponseInfo, string>(data.Item1, data.Item2.Length > 0 ? this._encoding.GetString(data.Item2) : "{}");
        }

        public async Task<Tuple<ResponseInfo, string>> UploadAsync(string url, string body, string method, Dictionary<string, string> headers = null)
        {
            Tuple<ResponseInfo, byte[]> data = await this.UploadRawAsync(url, body, method, headers).ConfigureAwait(false);
            return new Tuple<ResponseInfo, string>(data.Item1, data.Item2.Length > 0 ? this._encoding.GetString(data.Item2) : "{}");
        }

        public Tuple<ResponseInfo, byte[]> UploadRaw(string url, string body, string method, Dictionary<string, string> headers = null)
        {
            if (headers != null)
            {
                this.AddHeaders(headers);
            }

            HttpRequestMessage message = new HttpRequestMessage(new HttpMethod(method), url)
            {
                Content = new StringContent(body, this._encoding)
            };
            using (HttpResponseMessage response = Task.Run(() => this._client.SendAsync(message)).Result)
            {
                return new Tuple<ResponseInfo, byte[]>(new ResponseInfo
                {
                    StatusCode = response.StatusCode,
                    Headers = ConvertHeaders(response.Headers)
                }, Task.Run(() => response.Content.ReadAsByteArrayAsync()).Result);
            }
        }

        public async Task<Tuple<ResponseInfo, byte[]>> UploadRawAsync(string url, string body, string method, Dictionary<string, string> headers = null)
        {
            if (headers != null)
            {
                this.AddHeaders(headers);
            }

            HttpRequestMessage message = new HttpRequestMessage(new HttpMethod(method), url)
            {
                Content = new StringContent(body, this._encoding)
            };
            using (HttpResponseMessage response = await this._client.SendAsync(message))
            {
                return new Tuple<ResponseInfo, byte[]>(new ResponseInfo
                {
                    StatusCode = response.StatusCode,
                    Headers = ConvertHeaders(response.Headers)
                }, await response.Content.ReadAsByteArrayAsync());
            }
        }

        public Tuple<ResponseInfo, T> UploadJson<T>(string url, string body, string method, Dictionary<string, string> headers = null)
        {
            Tuple<ResponseInfo, string> response = this.Upload(url, body, method, headers);
            try
            {
                return new Tuple<ResponseInfo, T>(response.Item1, JsonConvert.DeserializeObject<T>(response.Item2, this.JsonSettings));
            }
            catch (JsonException)
            {
                return new Tuple<ResponseInfo, T>(response.Item1, JsonConvert.DeserializeObject<T>(UnknownErrorJson, this.JsonSettings));
            }
        }

        public async Task<Tuple<ResponseInfo, T>> UploadJsonAsync<T>(string url, string body, string method, Dictionary<string, string> headers = null)
        {
            Tuple<ResponseInfo, string> response = await this.UploadAsync(url, body, method, headers).ConfigureAwait(false);
            try
            {
                return new Tuple<ResponseInfo, T>(response.Item1, JsonConvert.DeserializeObject<T>(response.Item2, this.JsonSettings));
            }
            catch (JsonException)
            {
                return new Tuple<ResponseInfo, T>(response.Item1, JsonConvert.DeserializeObject<T>(UnknownErrorJson, this.JsonSettings));
            }
        }

        public void Dispose()
        {
            this._client.Dispose();
            GC.SuppressFinalize(this);
        }

        private static WebHeaderCollection ConvertHeaders(HttpResponseHeaders headers)
        {
            WebHeaderCollection newHeaders = new WebHeaderCollection();
            foreach (KeyValuePair<string, IEnumerable<string>> headerPair in headers)
            {
                foreach (string headerValue in headerPair.Value)
                {
                    newHeaders.Add(headerPair.Key, headerValue);
                }
            }
            return newHeaders;
        }

        private void AddHeaders(Dictionary<string,string> headers)
        {
            this._client.DefaultRequestHeaders.Clear();
            foreach (KeyValuePair<string, string> headerPair in headers)
            {
                this._client.DefaultRequestHeaders.TryAddWithoutValidation(headerPair.Key, headerPair.Value);
            }
        }

        private static HttpClientHandler CreateClientHandler(ProxyConfig proxyConfig = null)
        {
            HttpClientHandler clientHandler = new HttpClientHandler
            {
                PreAuthenticate = false,
                UseDefaultCredentials = true,
                UseProxy = false
            };

            if (string.IsNullOrWhiteSpace(proxyConfig?.Host)) return clientHandler;
            WebProxy proxy = proxyConfig.CreateWebProxy();
            clientHandler.UseProxy = true;
            clientHandler.Proxy = proxy;
            clientHandler.UseDefaultCredentials = proxy.UseDefaultCredentials;
            clientHandler.PreAuthenticate = proxy.UseDefaultCredentials;

            return clientHandler;
        }
    }
}