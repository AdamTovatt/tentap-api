using System.ComponentModel.DataAnnotations;

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
