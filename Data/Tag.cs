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
        public int ExerciseCount { get { if (_exerciseCount != null) return (int)_exerciseCount; return Exercises.Count(); } }

        private int? _exerciseCount;

        public Tag() { }

        public Tag(long exerciseCount)
        {
            _exerciseCount = (int)exerciseCount;
        }
    }
}
