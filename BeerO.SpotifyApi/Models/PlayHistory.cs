using System;
using Newtonsoft.Json;

namespace BeerO.SpotifyApi.Models
{
    public class PlayHistory : BasicModel
    {
        [JsonProperty("track")]
        public SimpleTrack Track { get; set; }

        [JsonProperty("played_at")]
        public DateTime PlayedAt { get; set; }

        [JsonProperty("context")]
        public Context Context { get; set; }
    }
}