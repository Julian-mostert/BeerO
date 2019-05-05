using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using BeerO.SpotifyApi;
using BeerO.SpotifyApi.Enums;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Modules;

namespace BeerO.SpotifyAuth
{
    public abstract class SpotifyAuthServer<T>
    {
        public string ClientId { get; set; }
        public string ServerUri { get; set; }
        public string RedirectUri { get; set; }
        public string State { get; set; }
        public Scope Scope { get; set; }
        public bool ShowDialog { get; set; }

        private readonly string _folder;
        private readonly string _type;
        private WebServer _server;
        protected CancellationTokenSource _serverSource;

        public delegate void OnAuthReceived(object sender, T payload);
        public event OnAuthReceived AuthReceived;

        internal static readonly Dictionary<string, SpotifyAuthServer<T>> Instances = new Dictionary<string, SpotifyAuthServer<T>>();

        internal SpotifyAuthServer(string type, string folder, string redirectUri, string serverUri, Scope scope = Scope.None, string state = "")
        {
            this._type = type;
            this._folder = folder;
            this.ServerUri = serverUri;
            this.RedirectUri = redirectUri;
            this.Scope = scope;
            this.State = string.IsNullOrEmpty(state) ? string.Join("", Guid.NewGuid().ToString("n").Take(8)) : state;
        }

        public void Start()
        {
            Instances.Add(this.State, this);
            this._serverSource = new CancellationTokenSource();

            this._server = WebServer.Create(this.ServerUri);
            this._server.RegisterModule(new WebApiModule());
            this.AdaptWebServer(this._server);
            this._server.RegisterModule(new ResourceFilesModule(Assembly.GetExecutingAssembly(), $"BeerO.SpotifyAuth.Resources.{this._folder}"));
#pragma warning disable 4014
            this._server.RunAsync(this._serverSource.Token);
#pragma warning restore 4014
        }

        public virtual string GetUri()
        {
            StringBuilder builder = new StringBuilder("https://accounts.spotify.com/authorize/?");
            builder.Append("client_id=" + this.ClientId);
            builder.Append($"&response_type={this._type}");
            builder.Append("&redirect_uri=" + this.RedirectUri);
            builder.Append("&state=" + this.State);
            builder.Append("&scope=" + this.Scope.GetStringAttribute(" "));
            builder.Append("&show_dialog=" + this.ShowDialog);
            return Uri.EscapeUriString(builder.ToString());
        }
            
        public void Stop(int delay = 2000)
        {
            if (this._serverSource == null) return;
            this._serverSource.CancelAfter(delay);
            Instances.Remove(this.State);
        }

        public void OpenBrowser()
        {
            string uri = this.GetUri();
            AuthUtil.OpenBrowser(uri);
        }

        internal void TriggerAuth(T payload)
        {
            this.AuthReceived?.Invoke(this, payload);
        }

        internal static SpotifyAuthServer<T> GetByState(string state)
        {
            return Instances.TryGetValue(state, out SpotifyAuthServer<T> auth) ? auth : null;
        }

        protected abstract void AdaptWebServer(WebServer webServer);
    }
}