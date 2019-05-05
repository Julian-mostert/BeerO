using Newtonsoft.Json;

namespace BeerO.SpotifyApi.Models
{
    public class CategoryList : BasicModel
    {
        [JsonProperty("categories")]
        public Paging<Category> Categories { get; set; }
    }
}