using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TentaPApi.Data
{
    public class Exercise
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public float Score { get; set; }
        public Source Source { get; set; }
        public ExerciseImage Image { get; set; }
        public ExerciseImage SolutionImage { get; set; }
    }
}
