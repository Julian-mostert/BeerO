using Newtonsoft.Json;

namespace BeerO.SpotifyApi.Models
{
    public class CategoryPlaylist : BasicModel
    {
        [JsonProperty("playlists")]
        public Paging<SimplePlaylist> Playlists { get; set; }
    }
}