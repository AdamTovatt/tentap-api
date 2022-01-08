using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TentaPApi.Data
{
    public record AddExerciseInput
    (
        int Number,
        int SourceId,
        int CourseId,
        int ModuleId,
        int TagId
    );

    public record AddExerciseImageInput
    (
        string Base64
    );
}
