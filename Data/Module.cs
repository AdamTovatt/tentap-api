using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TentaPApi.Data
{
    public class Module
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual List<Tag> Tags { get; set; }

        public Module() { }
    }
}
