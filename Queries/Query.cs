using HotChocolate;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TentaPApi.Data;

namespace TentaPApi.Queries
{
    public class Query
    {
        public IQueryable<Course> GetCourses([Service] ApplicationDbContext database)
        {
            return database.Course.Include(x => x.Modules);
        }

        public IQueryable<Course> GetCourse([Service] ApplicationDbContext database, int courseId)
        {
            return database.Course.Where(x => x.Id == courseId).Include(x => x.Modules).ThenInclude(x => x.Tags).ThenInclude(x => x.Exercises);
        }

        public IQueryable<Exercise> GetExercise([Service] ApplicationDbContext database, int exerciseId)
        {
            return database.Exercise.Where(x => x.Id == exerciseId).Include(x => x.Image).Include(x => x.SolutionImage).Include(x => x.Source);
        }
    }
}
