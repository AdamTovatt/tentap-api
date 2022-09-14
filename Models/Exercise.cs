using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using TentaPApi.Helpers;
using TentaPApi.Models;

namespace TentaPApi.Data
{
    public class Exercise
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("difficulty")]
        public Difficulty Difficulty { get; set; }

        [JsonProperty("module")]
        public Module Module;

        [JsonProperty("problem")]
        public CloudinaryImage ProblemImage { get; set; }

        [JsonProperty("solution")]
        public CloudinaryImage SolutionImage { get; set; }

        [JsonProperty("source")]
        public Source Source { get; set; }

        public async Task<List<DeletionResult>> DestroyImagesIfExistsAsync(Cloudinary cloudinary)
        {
            List<DeletionResult> deletionResults = new List<DeletionResult>();

            if (SolutionImage != null)
                deletionResults.Add(await cloudinary.DestroyAsync(new DeletionParams(SolutionImage.Id)));

            if (ProblemImage != null)
                deletionResults.Add(await cloudinary.DestroyAsync(new DeletionParams(ProblemImage.Id)));

            return deletionResults;
        }

        public static Exercise FromReader(NpgsqlDataReader reader)
        {
            Exercise result = new Exercise();
            result.Id = (int)reader["id"];
            result.Difficulty = (Difficulty)reader["difficulty"];
            result.Module = new Module((int)reader["module_id"], reader["name"] as string);
            result.Source = new Source() { Id = (int)reader["source_id"], Author = reader["author"] as string, Course = new Course() { Id = (int)reader["course_id"] }, Date = (DateTime)reader["source_date"] };

            CloudinaryImage problemImage = new CloudinaryImage() { Url = reader["problem_image"] as string };
            CloudinaryImage solutionImage = new CloudinaryImage() { Url = reader["solution_image"] as string };

            return result;
        }
    }
}
