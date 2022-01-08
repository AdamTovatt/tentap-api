using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TentaPApi.Data
{
    public class ExerciseImage
    {
        public int Id { get; set; }

        [Required]
        public byte[] Data { get; set; }
    }
}
