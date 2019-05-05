using System.Collections.Generic;
using Newtonsoft.Json;

namespace BeerO.SpotifyApi.Models
{
    public class RecommendationSeedGenres : BasicModel
    {
         [JsonProperty("genres")]
         public List<string> Genres { get; set; }
    }
}