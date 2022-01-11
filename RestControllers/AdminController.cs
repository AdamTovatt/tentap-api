using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using HotChocolate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sakur.WebApiUtilities.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TentaPApi.Data;
using TentaPApi.Helpers;
using TentaPApi.RequestBodies;
using WebApiUtilities.Helpers;

namespace TentaPApi.RestControllers
{
    [Route("[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("exercise/upload")]
        public async Task<IActionResult> Upload([FromBody] ExerciseUploadBody body)
        {
            string mode = "none";

            if (!body.Valid)
                return new ApiResponse(body.GetInvalidBodyMessage(), System.Net.HttpStatusCode.BadRequest);

            Source source = _context.Source.Where(x => x.Id == body.SourceId).FirstOrDefault();
            if (source == null)
                return new ApiResponse("Invalid sourceId: " + body.SourceId, System.Net.HttpStatusCode.BadRequest);

            Exercise exercise = new Exercise()
            {
                Number = body.Number,
                Source = source
            };

            if (body.Id > 0)
                exercise.Id = body.Id;

            Tag tag = _context.Course
                        .Include(x => x.Modules)
                        .ThenInclude(x => x.Tags)
                        .ThenInclude(x => x.Exercises)
                        .Where(x => x.Id == body.CourseId).SingleOrDefault().Modules
                        .Where(x => x.Id == body.ModuleId).SingleOrDefault().Tags
                        .Where(x => x.Id == body.TagId).SingleOrDefault();

            if (body.Id > 0 && tag.Exercises.Where(x => x.Id == exercise.Id).FirstOrDefault() != null)
            {
                exercise = tag.Exercises.Where(x => x.Id == exercise.Id).FirstOrDefault();
                mode = "update";
            }
            else
            {
                tag.Exercises.Add(exercise);
                mode = "create";
            }

            exercise.Number = body.Number;

            await _context.SaveChangesAsync();

            Cloudinary cloudinary = CloudinaryHelper.GetCloudinary();

            List<DeletionResult> deletionResults = await exercise.DestroyImagesIfExistsAsync(cloudinary);

            ImageUploadParams questionImage = new ImageUploadParams();
            questionImage.File = new FileDescription(string.Format("q{0}", exercise.Id), new MemoryStream(Convert.FromBase64String(body.ExerciseImageData)));
            ImageUploadResult questionUploadResult = await cloudinary.UploadAsync(questionImage);

            ImageUploadParams solutionImage = new ImageUploadParams();
            solutionImage.File = new FileDescription(string.Format("s{0}", exercise.Id), new MemoryStream(Convert.FromBase64String(body.SolutionImageData)));
            ImageUploadResult solutionUploadResult = await cloudinary.UploadAsync(solutionImage);

            exercise.SolutionImageUrl = solutionUploadResult.Url.ToString();
            exercise.ExerciseImageUrl = questionUploadResult.Url.ToString();
            exercise.SolutionImageId = solutionUploadResult.PublicId;
            exercise.ExerciseImageId = questionUploadResult.PublicId;
            await _context.SaveChangesAsync();

            return new ApiResponse(new { id = exercise.Id, exerciseUrl = questionUploadResult.Url, solutionUrl = solutionUploadResult.Url, mode, deletionResults });
        }

        [HttpDelete("exercise/remove")]
        public async Task<IActionResult> Remove(int id)
        {
            Exercise exercise = _context.Exercise.Where(x => x.Id == id).SingleOrDefault();

            if (exercise == null)
                return new ApiResponse("No such exercise, id: " + id, System.Net.HttpStatusCode.BadRequest);

            await exercise.DestroyImagesIfExistsAsync(CloudinaryHelper.GetCloudinary());

            _context.Remove(exercise);
            await _context.SaveChangesAsync();

            return new ApiResponse();
        }
    }
}
