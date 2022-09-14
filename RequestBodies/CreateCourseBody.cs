using Newtonsoft.Json;
using Sakur.WebApiUtilities.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TentaPApi.RequestBodies
{
    public class CreateCourseBody : RequestBody
    {
        [JsonProperty("code")]
        public string Code { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }

        public override bool Valid { get { return !string.IsNullOrEmpty(Code) && !string.IsNullOrEmpty(Name); } }
    }
}
