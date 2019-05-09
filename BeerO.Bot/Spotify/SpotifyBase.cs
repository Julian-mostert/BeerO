using System.IO;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;

namespace BeerOBot.ConsoleApp.Spotify
{
    public class SpotifyBase : ISpotifyBase
    {
        public  SpotifyWebAPI  SpotifyWebApi      { get; set; }
        public  PrivateProfile SpotifyUserProfile { get; set; }
        public  Token          spotifyToken       { get; set; }
        private bool           isAuthenticated    { get; set; } = false;
        private string         AppPath            { get; set; }

        public SpotifyBase()
        {
            this.AppPath = $"{Directory.GetCurrentDirectory()}\\Token.Json";
            this.spotifyToken = this.GetSpotifyToken();
            this.SpotifyWebApi = this.CreateSpotifyWebApi(this.spotifyToken);
            this.SpotifyUserProfile = this.GetPrivateProfile(this.SpotifyWebApi);
        }

        private Token GetSpotifyToken()
        {
            Token token = null;
            bool tokenFileExists = File.Exists(this.AppPath);

            if (tokenFileExists)
            {
                token = this.RefreshToken();
            }
            else
            {
                token = this.GetAuthenticationToken();
            }

            return token;
        }

        private Token GetAuthenticationToken()
        {
            Token token;
            bool hasRun = false;
            while (!this.isAuthenticated)
            {
                if (!hasRun)
                {
                    this.AuthorizeSpotifyToken();
                    hasRun = true;
                }
            }

            token = this.GetTokenFromFile();
            return token;
        }

        public Token RefreshToken()
        {
            this.spotifyToken= this.GetTokenFromFile();
            AuthorizationCodeAuth auth = this.CreateAuthorization();
            Token refreshedToken = auth.RefreshToken(this.spotifyToken.RefreshToken).Result;
            refreshedToken.RefreshToken = this.spotifyToken.RefreshToken;

            this.SpotifyWebApi = this.CreateSpotifyWebApi(this.spotifyToken);

            this.CreateTokenFile(refreshedToken);
            this.spotifyToken = refreshedToken;
            
            return refreshedToken;
        }

        private Token GetTokenFromFile()
        {
            Token token;
            using (StreamReader r = new StreamReader(this.AppPath))
            {
                string json = r.ReadToEnd();
                token = JsonConvert.DeserializeObject<Token>(json);
            }

            return token;
        }

        private void AuthorizeSpotifyToken()
        {
            AuthorizationCodeAuth auth = this.CreateAuthorization();

            auth.AuthReceived += async (sender, payload) =>
            {
                auth.Stop();
                Token token = await auth.ExchangeCode(payload.Code);
                this.CreateTokenFile(token);
                this.isAuthenticated = true;
            };

            auth.Start();
            auth.OpenBrowser();
        }

        private AuthorizationCodeAuth CreateAuthorization()
        {
            string _clientId = "d561c6c5f0be4ff29a5b64ceb9e703da";
            string _secretId = "534e8567260842519f58461101208a83";
            AuthorizationCodeAuth auth =
                new AuthorizationCodeAuth(_clientId, 
                                          _secretId, 
                                          "http://localhost:4002", 
                                          "http://localhost:4002",
                                          Scope.PlaylistReadPrivate | Scope.PlaylistReadCollaborative | Scope.PlaylistModifyPublic | Scope.UserReadCurrentlyPlaying | Scope.UserReadPlaybackState
                                          );
            return auth;
        }

        private void CreateTokenFile(Token token)
        {
            using (StreamWriter file = File.CreateText(this.AppPath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, token);
            }
        }

        private SpotifyWebAPI CreateSpotifyWebApi(Token token)
        {
            SpotifyWebAPI spotify = new SpotifyWebAPI
            {
                TokenType = token.TokenType,
                AccessToken = token.AccessToken
            };

            return spotify;
        }

        private PrivateProfile GetPrivateProfile(SpotifyWebAPI spotifyWebApi)
        {
            PrivateProfile privateProfile = spotifyWebApi.GetPrivateProfile();
            return privateProfile;
        }
    }
}