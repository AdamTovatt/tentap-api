using Newtonsoft.Json;
using Sakur.WebApiUtilities.BaseClasses;
using TentaPApi.Data;
using TentaPApi.Models;

namespace TentaPApi.RequestBodies
{
    public class CreateExerciseBody : RequestBody
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
        public string ProblemImageData { get; set; }

        [JsonProperty("solutionImageData")]
        public string SolutionImageData { get; set; }

        public override bool Valid { get { return SourceId != 0; } }

        public CreateExerciseBody() { }

        public static CreateExerciseBody FromJson(string json)
        {
            return JsonConvert.DeserializeObject<CreateExerciseBody>(json);
        }

        public Exercise GetExercise()
        {
            return new Exercise()
            {
                Difficulty = Difficulty,
                Id = Id,
                Module = new Module() { Id = ModuleId },
                Source = new Source() { Id = SourceId }
            };
        }
    }
}
