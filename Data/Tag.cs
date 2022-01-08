using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TentaPApi.Data
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual List<Exercise> Exercises { get; set; }

        [NotMapped]
        public int ExerciseCount { get { return Exercises.Count(); } }
    }
}
