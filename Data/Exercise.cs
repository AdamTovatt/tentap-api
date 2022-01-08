using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TentaPApi.Data
{
    public class Exercise
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public float Score { get; set; }

        [Required]
        public virtual Source Source { get; set; }

        [Required]
        public virtual ExerciseImage Image { get; set; }

        [Required]
        public virtual ExerciseImage SolutionImage { get; set; }
    }
}
