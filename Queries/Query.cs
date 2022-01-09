using HotChocolate;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TentaPApi.Data;
using TentaPApi.Managers;

namespace TentaPApi.Queries
{
    public class Query
    {
        public IQueryable<Course> GetCourses([Service] ApplicationDbContext database)
        {
            return database.Course.Include(x => x.Modules);
        }

        public async Task<Course> GetCourse([Service] ApplicationDbContext database, int courseId)
        {
            int time = 0;
            string method = "";
            Course result;

            if (DateTime.UtcNow.Millisecond % 2 == 0)
            {
                method = "ORM";
                Stopwatch stopwatch = Stopwatch.StartNew();
                result = database.Course.Where(x => x.Id == courseId).Include(x => x.Modules).ThenInclude(x => x.Tags).ThenInclude(x => x.Exercises).ToList().First();
                stopwatch.Stop();
                time += (int)stopwatch.ElapsedMilliseconds;
            }
            else
            {
                method = "TSQL";
                Stopwatch stopwatch = Stopwatch.StartNew();
                result = await new DatabaseManager().GetCourse(courseId);
                stopwatch.Stop();
                time += (int)stopwatch.ElapsedMilliseconds;
            }

            result.DebugInfo = "Method: " + method + " Time: " + time + " ms";
            return result;
        }

        public IQueryable<Exercise> GetExercise([Service] ApplicationDbContext database, int exerciseId)
        {
            return database.Exercise.Where(x => x.Id == exerciseId).Include(x => x.Image).Include(x => x.SolutionImage).Include(x => x.Source);
        }
    }
}
