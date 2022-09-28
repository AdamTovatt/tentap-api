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
        public string Url { get { return url; } set { url = value.Replace("http://", "https://"); } }
        private string url;
        
        [JsonProperty("id")]
        public string Id { get { if (_id == null) GenerateId(); return _id; } }
        private string _id;

        private void GenerateId()
        {
            if (Url == null)
                return;

            _id = Url.Split("/").Last().Split(".")[0];
        }

        public bool IsNull()
        {
            return Url != null;
        }
    }
}
