using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using HotChocolate;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using TentaPApi.Helpers;

namespace TentaPApi.Data
{
    public class Exercise
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public float Score { get; set; }
        public string ExerciseImageUrl { get; set; }
        public string SolutionImageUrl { get; set; }
        public string ExerciseImageId { get; set; }
        public string SolutionImageId { get; set; }
        public virtual Source Source { get; set; }

        [GraphQLIgnore]
        public async Task<List<DeletionResult>> DestroyImagesIfExistsAsync(Cloudinary cloudinary)
        {
            List<DeletionResult> deletionResults = new List<DeletionResult>();

            if (SolutionImageUrl != null || ExerciseImageUrl != null) //if images already exist we remove them
            {
                if (SolutionImageUrl != null && SolutionImageId != null)
                {
                    deletionResults.Add(await cloudinary.DestroyAsync(new DeletionParams(SolutionImageId)));
                }

                if (ExerciseImageUrl != null && ExerciseImageId != null)
                {
                    deletionResults.Add(await cloudinary.DestroyAsync(new DeletionParams(ExerciseImageId)));
                }
            }

            return deletionResults;
        }
    }
}
