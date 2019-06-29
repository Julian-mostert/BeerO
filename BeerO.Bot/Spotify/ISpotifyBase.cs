using SpotifyAPI.Web;
using SpotifyAPI.Web.Models;

namespace BeerO.Bot.Spotify
{
    public interface ISpotifyBase
    {
        SpotifyWebAPI  SpotifyWebApi      { get; set; }
        PrivateProfile SpotifyUserProfile { get; set; }
        Token spotifyToken { get; set; }

        Token RefreshToken();
    }
}