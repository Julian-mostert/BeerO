using System.Collections.Generic;
using Newtonsoft.Json;

namespace BeerO.SpotifyApi.Models
{
    public class SeveralTracks : BasicModel
    {
        [JsonProperty("tracks")]
        public List<FullTrack> Tracks { get; set; }
    }
}