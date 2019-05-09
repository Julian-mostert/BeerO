using SpotifyAPI.Web;
using SpotifyAPI.Web.Models;

namespace BeerOBot.ConsoleApp.Spotify
{
    public interface ISpotifyBase
    {
        SpotifyWebAPI  SpotifyWebApi      { get; set; }
        PrivateProfile SpotifyUserProfile { get; set; }
        Token spotifyToken { get; set; }

        Token RefreshToken();
    }
}