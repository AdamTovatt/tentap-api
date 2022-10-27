using Newtonsoft.Json;
using Sakur.WebApiUtilities.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TentaPApi.RequestBodies
{
    public class GetNextExerciseBody : RequestBody
    {
        [JsonProperty("courseId")]
        public int CourseId { get; set; }

        [JsonProperty("includeEasy")]
        public bool IncludeEasy { get; set; }

        [JsonProperty("includeMedium")]
        public bool IncludeMedium { get; set; }

        [JsonProperty("includeHard")]
        public bool IncludeHard { get; set; }

        [JsonProperty("excludeModules")]
        public List<int> ExcludeModules { get; set; }

        public override bool Valid { get { return CourseId != 0 && (IncludeEasy || IncludeMedium || IncludeHard); } }
    }
}
