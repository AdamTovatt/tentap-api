using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TentaPApi.Data;

namespace TentaPApi.Queries
{
    public class Query
    {
        public List<Course> GetCourses()
        {
            return new List<Course>()
            {
                 new Course() { Id = 1, Name = "Linjalg" },
                 new Course() { Id = 2, Name = "Elovåg" },
            };
        }
    }
}
