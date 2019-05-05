using System.Threading.Tasks;
using BeerO.SpotifyApi.Enums;
using BeerO.SpotifyApi.Models;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Constants;
using Unosquare.Labs.EmbedIO.Modules;

namespace BeerO.SpotifyAuth
{
    public class ImplicitGrantAuth : SpotifyAuthServer<Token>
    {
        public ImplicitGrantAuth(string clientId, string redirectUri, string serverUri, Scope scope = Scope.None, string state = "") :
            base("token", "ImplicitGrantAuth", redirectUri, serverUri, scope, state)
        {
            this.ClientId = clientId;
        }

        protected override void AdaptWebServer(WebServer webServer)
        {
            webServer.Module<WebApiModule>().RegisterController<ImplicitGrantAuthController>();
        }
    }

    public class ImplicitGrantAuthController : WebApiController
    {
        [WebApiHandler(HttpVerbs.Get, "/auth")]
        public Task<bool> GetAuth()
        {
            string state = this.Request.QueryString["state"];
            SpotifyAuthServer<Token> auth = ImplicitGrantAuth.GetByState(state);
            if (auth == null)
                return this.Response.StringResponseAsync(
                    $"Failed - Unable to find auth request with state \"{state}\" - Please retry");

            Token token;
            string error = this.Request.QueryString["error"];
            if (error == null)
            {
                string accessToken = this.Request.QueryString["access_token"];
                string tokenType = this.Request.QueryString["token_type"];
                string expiresIn = this.Request.QueryString["expires_in"];
                token = new Token
                {
                    AccessToken = accessToken,
                    ExpiresIn = double.Parse(expiresIn),
                    TokenType = tokenType
                };
            }
            else
            {
                token = new Token
                {
                    Error = error
                };
            }

            Task.Factory.StartNew(() => auth.TriggerAuth(token));
            return this.Response.StringResponseAsync("<html><script type=\"text/javascript\">window.close();</script>OK - This window can be closed now</html>");
        }

        public ImplicitGrantAuthController(IHttpContext context) : base(context)
        {
        }
    }
}
