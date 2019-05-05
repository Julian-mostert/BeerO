using System.Net;
using Newtonsoft.Json;

namespace BeerO.SpotifyApi.Models
{
    public abstract class BasicModel
    {
        [JsonProperty("error")]
        public Error Error { get; set; }

        private ResponseInfo _info;

        public bool HasError() => this.Error != null;

        internal void AddResponseInfo(ResponseInfo info) => this._info = info;

        public string Header(string key) => this._info.Headers?.Get(key);

        public WebHeaderCollection Headers() => this._info.Headers;

        public HttpStatusCode StatusCode() => this._info.StatusCode;
    }
}