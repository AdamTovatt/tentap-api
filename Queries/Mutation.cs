using HotChocolate;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TentaPApi.Data;

namespace TentaPApi.Queries
{
    public class Mutation
    {
        public async Task<Exercise> AddExerciseAsync(AddExerciseInput exerciseInput, AddExerciseImageInput exerciseImage, AddExerciseImageInput solutionImage, [Service] ApplicationDbContext database)
        {
            Exercise exercise = new Exercise()
            {
                Number = exerciseInput.Number,
                Source = database.Source.Where(x => x.Id == exerciseInput.SourceId).FirstOrDefault(),
                Image = new ExerciseImage() { Data = Convert.FromBase64String(exerciseImage.Base64) },
                SolutionImage = new ExerciseImage() { Data = Convert.FromBase64String(solutionImage.Base64) }
            };

            database.Course
                .Include(x => x.Modules)
                .ThenInclude(x => x.Tags)
                .ThenInclude(x => x.Exercises)
                .Where(x => x.Id == exerciseInput.CourseId).SingleOrDefault().Modules
                .Where(x => x.Id == exerciseInput.ModuleId).SingleOrDefault().Tags
                .Where(x => x.Id == exerciseInput.TagId).SingleOrDefault().Exercises
                .Add(exercise);

            await database.SaveChangesAsync();

            return exercise;
        }
    }
}
