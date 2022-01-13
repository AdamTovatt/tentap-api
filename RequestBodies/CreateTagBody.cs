using Sakur.WebApiUtilities.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TentaPApi.RequestBodies
{
    public class CreateTagBody : RequestBody
    {
        public int ModuleId { get; set; }
        public string TagName { get; set; }

        public override bool Valid { get { return ModuleId != 0 && !string.IsNullOrEmpty(TagName); } }
    }
}
