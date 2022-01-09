using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using TentaPApi.Helpers;

namespace TentaPApi.Data
{
    public class ExerciseImage
    {
        public int Id { get; set; }

        [Required]
        public byte[] Data { get; set; }

        [NotMapped]
        public string Url { get { return string.Format(EnvironmentHelper.GetEnvironmentVariable("IMAGE_URL_BASE"), Id); } }
    }
}
