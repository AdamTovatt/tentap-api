using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TentaPApi.Models
{
    public class CloudinaryImage
    {
        [JsonProperty("url")]
        public string Url { get; set; }
        
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
