using Newtonsoft.Json;

namespace BeerO.SpotifyApi.Models
{
    public class Snapshot : BasicModel
    {
        [JsonProperty("snapshot_id")]
        public string SnapshotId { get; set; }
    }
}