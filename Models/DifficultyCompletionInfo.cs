using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TentaPApi.Models
{
    public class DifficultyCompletionInfo
    {
        [JsonProperty("completed")]
        public int Completed { get; set; }
        
        [JsonProperty("total")]
        public int Total { get; set; }
    }
}
