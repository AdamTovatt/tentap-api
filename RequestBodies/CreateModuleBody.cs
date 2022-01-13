using Sakur.WebApiUtilities.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TentaPApi.RequestBodies
{
    public class CreateModuleBody : RequestBody
    {
        public int CourseId { get; set; }
        public string ModuleName { get; set; }

        public override bool Valid { get { return CourseId != 0 && !string.IsNullOrEmpty(ModuleName); } }
    }
}
