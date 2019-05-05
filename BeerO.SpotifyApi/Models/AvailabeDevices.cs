using System.Collections.Generic;
using Newtonsoft.Json;

namespace BeerO.SpotifyApi.Models
{
    public class AvailabeDevices : BasicModel
    {
        [JsonProperty("devices")]
        public List<Device> Devices { get; set; }
    }
}