using Newtonsoft.Json;
using Sakur.WebApiUtilities.BaseClasses;
using TentaPApi.Models;

namespace TentaPApi.RequestBodies
{
    public class UploadExerciseBody : RequestBody
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("difficulty")]
        public Difficulty Difficulty { get; set; }

        [JsonProperty("sourceId")]
        public int SourceId { get; set; }

        [JsonProperty("moduleId")]
        public int ModuleId { get; set; }

        [JsonProperty("exerciseImageData")]
        public string ExerciseImageData { get; set; }

        [JsonProperty("solutionImageData")]
        public string SolutionImageData { get; set; }

        public override bool Valid { get { return SourceId != 0 && !string.IsNullOrEmpty(ExerciseImageData) && !string.IsNullOrEmpty(SolutionImageData); } }

        public UploadExerciseBody() { }

        public static UploadExerciseBody FromJson(string json)
        {
            return JsonConvert.DeserializeObject<UploadExerciseBody>(json);
        }
    }
}
