using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sakur.WebApiUtilities.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TentaPApi.Data;
using TentaPApi.Helpers;
using TentaPApi.Managers;
using TentaPApi.RequestBodies;
using WebApiUtilities.Helpers;

namespace TentaPApi.RestControllers
{
    [Route("[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        [Authorize(Roles = "1,2")]
        [HttpPost("source/create")]
        public async Task<IActionResult> CreateSource([FromBody] CreateSourceBody body)
        {
            try
            {
                if (!body.Valid)
                    return new ApiResponse(body.GetInvalidBodyMessage(), HttpStatusCode.BadRequest);

                DatabaseManager database = new DatabaseManager(UserHelper.GetClaims(User).GetUserId());

                return new ApiResponse(await database.AddSourceAsync(body.GetSource()));
            }
            catch (ApiException exception)
            {
                return new ApiResponse(exception);
            }
        }

        [Authorize(Roles = "1,2")]
        [HttpPost("module/create")]
        public async Task<IActionResult> CreateModule([FromBody] CreateModuleBody body)
        {
            try
            {
                if (!body.Valid)
                    return new ApiResponse(body.GetInvalidBodyMessage(), HttpStatusCode.BadRequest);

                DatabaseManager database = new DatabaseManager(UserHelper.GetClaims(User).GetUserId());

                Course course = new Course() { Id = body.CourseId };
                Module module = new Module() { Name = body.ModuleName, Course = course };

                module = await database.AddModuleAsync(module);

                return new ApiResponse(module);
            }
            catch (ApiException exception)
            {
                return new ApiResponse(exception);
            }
        }

        [Authorize(Roles = "1,2")]
        [HttpPost("course/create")]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseBody body)
        {
            try
            {
                if (!body.Valid)
                    return new ApiResponse(body.GetInvalidBodyMessage(), HttpStatusCode.BadRequest);

                DatabaseManager database = new DatabaseManager(UserHelper.GetClaims(User).GetUserId());

                Course course = new Course() { Code = body.Code, Name = body.Name };
                course = await database.AddCourseAsync(course);

                return new ApiResponse(course);
            }
            catch (ApiException exception)
            {
                return new ApiResponse(exception);
            }
        }

        [Authorize(Roles = "1,2")]
        [HttpPost("exercise/create")]
        public async Task<IActionResult> Upload([FromBody] CreateExerciseBody body)
        {
            try
            {
                if (!body.Valid)
                    return new ApiResponse(body.GetInvalidBodyMessage(), HttpStatusCode.BadRequest);

                DatabaseManager database = new DatabaseManager(UserHelper.GetClaims(User).GetUserId());

                Source source = await database.GetSourceAsync(body.SourceId);

                if (source == null)
                    return new ApiResponse("Invalid sourceId: " + body.SourceId, HttpStatusCode.BadRequest);

                Module module = await database.GetModuleAsync(body.ModuleId);

                if (module == null)
                    return new ApiResponse("Invalid moduleId: " + body.ModuleId, HttpStatusCode.BadRequest);

                ExerciseImageUploader imageUploader = new ExerciseImageUploader(body.ProblemImageData, body.SolutionImageData);
                bool result = await imageUploader.UploadImagesAsync();

                if (!result)
                    return new ApiResponse("Error when uploading images", HttpStatusCode.InternalServerError);

                Exercise exercise = body.GetExercise();
                exercise.ProblemImage = imageUploader.ProblemImage;
                exercise.SolutionImage = imageUploader.SolutionImage;
                exercise.Source = source;
                exercise.Module = module;

                exercise = await database.AddExerciseAsync(exercise);

                return new ApiResponse(exercise);
            }
            catch (ApiException exception)
            {
                return new ApiResponse(exception);
            }
        }

        [Authorize(Roles = "1,2")]
        [HttpDelete("exercise/remove")]
        public async Task<IActionResult> Remove(int id)
        {
            try
            {
                DatabaseManager database = new DatabaseManager(UserHelper.GetClaims(User).GetUserId());
                Exercise exercise = await database.GetExerciseByIdAsync(id);

                if (exercise == null)
                    return new ApiResponse("No such exercise, id: " + id, HttpStatusCode.BadRequest);

                List<DeletionResult> deletionResults = await exercise.DestroyImagesIfExistsAsync(CloudinaryHelper.GetCloudinary());

                if (!await database.RemoveExerciseAsync(id))
                    return new ApiResponse("Error when removing exercise: " + id, HttpStatusCode.InternalServerError);

                return new ApiResponse("The exercise that had id " + id + " exists now only in our collective memory of it :'(");
            }
            catch (ApiException exception)
            {
                return new ApiResponse(exception);
            }
        }
    }
}
