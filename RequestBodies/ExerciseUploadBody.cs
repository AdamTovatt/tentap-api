using Newtonsoft.Json;
using Sakur.WebApiUtilities.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TentaPApi.RequestBodies
{
    public class ExerciseUploadBody : RequestBody
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("number")]
        public int Number { get; set; }

        [JsonProperty("sourceId")]
        public int SourceId { get; set; }

        [JsonProperty("courseId")]
        public int CourseId { get; set; }

        [JsonProperty("moduleId")]
        public int ModuleId { get; set; }

        [JsonProperty("tagId")]
        public int TagId { get; set; }

        [JsonProperty("exerciseImageData")]
        public string ExerciseImageData { get; set; }

        [JsonProperty("solutionImageData")]
        public string SolutionImageData { get; set; }

        public override bool Valid { get { return Number != 0 && SourceId != 0 && !string.IsNullOrEmpty(ExerciseImageData) && !string.IsNullOrEmpty(SolutionImageData); } }

        public ExerciseUploadBody() { }

        public static ExerciseUploadBody FromJson(string json)
        {
            return JsonConvert.DeserializeObject<ExerciseUploadBody>(json);
        }
    }
}
