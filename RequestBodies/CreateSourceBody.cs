using Newtonsoft.Json;
using Sakur.WebApiUtilities.BaseClasses;
using System;
using TentaPApi.Data;

namespace TentaPApi.RequestBodies
{
    public class CreateSourceBody : RequestBody
    {
        [JsonProperty("courseId")]
        public int CourseId { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        public override bool Valid { get { return CourseId != 0 && !string.IsNullOrEmpty(Author) && Date != DateTime.MinValue; } }

        public Source GetSource()
        {
            Course course = new Course() { Id = CourseId };
            return new Source() { Author = Author, Course = course, Date = Date };
        }
    }
}
