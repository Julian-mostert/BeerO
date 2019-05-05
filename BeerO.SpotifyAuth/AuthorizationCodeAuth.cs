using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using BeerO.SpotifyApi.Enums;
using BeerO.SpotifyApi.Models;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Constants;
using Unosquare.Labs.EmbedIO.Modules;

namespace BeerO.SpotifyAuth
{
    public class AuthorizationCodeAuth : SpotifyAuthServer<AuthorizationCode>
    {
        public string SecretId { get; set; }

        public AuthorizationCodeAuth(string redirectUri, string serverUri, Scope scope = Scope.None, string state = "")
            : base("code", "AuthorizationCodeAuth", redirectUri, serverUri, scope, state)
        {
        }

        public AuthorizationCodeAuth(string clientId, string secretId, string redirectUri, string serverUri, Scope scope = Scope.None, string state = "")
            : this(redirectUri, serverUri, scope, state)
        {
            this.ClientId = clientId;
            this.SecretId = secretId;
        }

        private bool ShouldRegisterNewApp()
        {
            return string.IsNullOrEmpty(this.SecretId) || string.IsNullOrEmpty(this.ClientId);
        }

        public override string GetUri()
        {
            return this.ShouldRegisterNewApp() ? $"{this.RedirectUri}/start.html#{this.State}" : base.GetUri();
        }

        protected override void AdaptWebServer(WebServer webServer)
        {
            webServer.Module<WebApiModule>().RegisterController<AuthorizationCodeAuthController>();
        }

        private string GetAuthHeader() => $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes(this.ClientId + ":" + this.SecretId))}";

        public async Task<Token> RefreshToken(string refreshToken)
        {
            List<KeyValuePair<string, string>> args = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", refreshToken)
            };

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", this.GetAuthHeader());
            HttpContent content = new FormUrlEncodedContent(args);

            HttpResponseMessage resp = await client.PostAsync("https://accounts.spotify.com/api/token", content);
            string msg = await resp.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Token>(msg);
        }
        public async Task<Token> ExchangeCode(string code)
        {
            List<KeyValuePair<string, string>> args = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", this.RedirectUri)
            };

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", this.GetAuthHeader());
            HttpContent content = new FormUrlEncodedContent(args);

            HttpResponseMessage resp = await client.PostAsync("https://accounts.spotify.com/api/token", content);
            string msg = await resp.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Token>(msg);
        }
    }

    public class AuthorizationCode
    {
        public string Code { get; set; }

        public string Error { get; set; }
    }

    internal class AuthorizationCodeAuthController : WebApiController
    {
        [WebApiHandler(HttpVerbs.Get, "/")]
        public Task<bool> GetEmpty()
        {
            string state = this.Request.QueryString["state"];
            AuthorizationCodeAuth.Instances.TryGetValue(state, out SpotifyAuthServer<AuthorizationCode> auth);

            string code = null;
            string error = this.Request.QueryString["error"];
            if (error == null)
                code = this.Request.QueryString["code"];

            Task.Factory.StartNew(() => auth?.TriggerAuth(new AuthorizationCode
            {
                Code = code,
                Error = error
            }));
            
            return this.Response.StringResponseAsync("OK - This window can be closed now");
        }

        [WebApiHandler(HttpVerbs.Post, "/")]
        public async Task<bool> PostValues()
        {
            Dictionary<string, object> formParams = await this.HttpContext.RequestFormDataDictionaryAsync();

            string state = (string) formParams["state"];
            AuthorizationCodeAuth.Instances.TryGetValue(state, out SpotifyAuthServer<AuthorizationCode> authServer);

            AuthorizationCodeAuth auth = (AuthorizationCodeAuth) authServer;
            auth.ClientId = (string) formParams["clientId"];
            auth.SecretId = (string) formParams["secretId"];

            string uri = auth.GetUri();
            return this.HttpContext.Redirect(uri, false);
        }

        public AuthorizationCodeAuthController(IHttpContext context) : base(context)
        {
        }
    }
}
