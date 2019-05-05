using System.Collections.Generic;
using Newtonsoft.Json;

namespace BeerO.SpotifyApi.Models
{
    public class SeveralArtists : BasicModel
    {
        [JsonProperty("artists")]
        public List<FullArtist> Artists { get; set; }
    }
}