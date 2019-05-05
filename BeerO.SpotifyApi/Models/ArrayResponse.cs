using System.Collections.Generic;

namespace BeerO.SpotifyApi.Models
{
    public class ListResponse<T> : BasicModel
    {
        public List<T> List { get; set; }
    }
}