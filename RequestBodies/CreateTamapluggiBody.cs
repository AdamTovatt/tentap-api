using Newtonsoft.Json;
using Sakur.WebApiUtilities.BaseClasses;

namespace TentaPApi.RequestBodies
{
    public class CreateTamapluggiBody : RequestBody
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("studyGoal")]
        public int StudyGoal { get; set; }

        [JsonProperty("breakDuration")]
        public int BreakDuration { get; set; }

        public override bool Valid { get { return !string.IsNullOrEmpty(Name) && StudyGoal != 0; } }
    }
}
