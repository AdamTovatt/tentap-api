using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using TentaPApi.Queries;

namespace TentaPApi.Data
{
    public class Course
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual List<Module> Modules { get; set; }
    }
}
