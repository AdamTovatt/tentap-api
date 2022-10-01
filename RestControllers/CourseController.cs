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
using TentaPApi.Models;
using TentaPApi.RequestBodies;
using WebApiUtilities.Helpers;

namespace TentaPApi.RestControllers
{
    [Route("[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet("getall")]
        public async Task<IActionResult> GetCourses(bool includeInactive)
        {
            try
            {
                DatabaseManager database = new DatabaseManager(UserHelper.GetClaims(User).GetUserId());

                return new ApiResponse(await database.GetCoursesAsync(includeInactive));
            }
            catch (ApiException exception)
            {
                return new ApiResponse(exception);
            }
        }

        [AllowAnonymous]
        [HttpGet("getSourcesByCourseId")]
        public async Task<IActionResult> GetSourcesByCourseId(int courseId)
        {
            try
            {
                if (courseId == 0)
                    return new ApiResponse("Missing queryParameter courseId, should be course id", HttpStatusCode.BadRequest);

                DatabaseManager database = new DatabaseManager(UserHelper.GetClaims(User).GetUserId());

                return new ApiResponse(await database.GetSourcesByCourseAsync(courseId));
            }
            catch (ApiException exception)
            {
                return new ApiResponse(exception);
            }
        }

        [AllowAnonymous]
        [HttpGet("getModulesByCourseId")]
        public async Task<IActionResult> GetModulesByCourseId(int courseId)
        {
            try
            {
                if (courseId == 0)
                    return new ApiResponse("Missing queryParameter courseId, should be course id", HttpStatusCode.BadRequest);

                DatabaseManager database = new DatabaseManager(UserHelper.GetClaims(User).GetUserId());

                return new ApiResponse(await database.GetModulesByCourseAsync(courseId));
            }
            catch (ApiException exception)
            {
                return new ApiResponse(exception);
            }
        }

        [AllowAnonymous]
        [HttpGet("get")]
        public async Task<IActionResult> GetCourse(int id)
        {
            try
            {
                if (id == 0)
                    return new ApiResponse("Missing queryParameter id, should be course id", HttpStatusCode.BadRequest);

                DatabaseManager database = new DatabaseManager(UserHelper.GetClaims(User).GetUserId());

                return new ApiResponse(await database.GetCourseAsync(id));
            }
            catch (ApiException exception)
            {
                return new ApiResponse(exception);
            }
        }

        [AllowAnonymous]
        [HttpGet("exercise/getAll")]
        public async Task<IActionResult> GetNextExercise(int courseId, bool onlyInactive)
        {
            try
            {
                if (courseId == 0)
                    return new ApiResponse("Missing courseId query parameter, should be id of course", HttpStatusCode.BadRequest);

                DatabaseManager database = new DatabaseManager(UserHelper.GetClaims(User).GetUserId());

                return new ApiResponse(await database.GetExercisesByCourseAsync(courseId, onlyInactive));
            }
            catch (ApiException exception)
            {
                return new ApiResponse(exception);
            }
        }

        [AllowAnonymous]
        [HttpGet("exercise/get")]
        public async Task<IActionResult> GetExerciseById(int exerciseId)
        {
            try
            {
                if (exerciseId == 0)
                    return new ApiResponse("Missing exerciseId query parameter, should be id of exercise", HttpStatusCode.BadRequest);

                DatabaseManager database = new DatabaseManager(UserHelper.GetClaims(User).GetUserId());

                return new ApiResponse(await database.GetExerciseByIdAsync(exerciseId));
            }
            catch (ApiException exception)
            {
                return new ApiResponse(exception);
            }
        }

        [AllowAnonymous]
        [HttpGet("exercise/getNext")]
        public async Task<IActionResult> GetNextExercise(int courseId, bool easy, bool medium, bool hard)
        {
            try
            {
                if (courseId == 0)
                    return new ApiResponse("Missing courseId query parameter, should be id of course", HttpStatusCode.BadRequest);

                List<Difficulty> difficultyList = new List<Difficulty>();
                if (easy) difficultyList.Add(Difficulty.Easy);
                if (medium) difficultyList.Add(Difficulty.Medium);
                if (hard) difficultyList.Add(Difficulty.Hard);

                if (difficultyList.Count == 0)
                    return new ApiResponse("Missing queryParameters for difficulty: easy, medium and hard. At least one needs to be true", HttpStatusCode.BadRequest);

                DatabaseManager database = new DatabaseManager(UserHelper.GetClaims(User).GetUserId());

                return new ApiResponse(await database.GetExerciseForUserAsync(difficultyList.ToArray(), courseId));
            }
            catch (ApiException exception)
            {
                return new ApiResponse(exception);
            }
        }

        [Authorize]
        [HttpGet("getCompletionInfo")]
        public async Task<IActionResult> CompleteExercise(int courseId)
        {
            try
            {
                if (courseId == 0)
                    return new ApiResponse("Missing courseId parameter in query parameters, should be int > 0", HttpStatusCode.BadRequest);

                DatabaseManager database = new DatabaseManager(UserHelper.GetClaims(User).GetUserId());

                return new ApiResponse(await database.GetCourseCompletionInfoAsync(courseId));
            }
            catch (ApiException exception)
            {
                return new ApiResponse(exception);
            }
        }

        [Authorize]
        [HttpPost("exercise/setCompleted")]
        public async Task<IActionResult> CompleteExercise(int exerciseId, [FromBody] GetNextExerciseBody body)
        {
            try
            {
                if (exerciseId == 0)
                    return new ApiResponse("Missing exerciseId query parameter, should be id of exercise", HttpStatusCode.BadRequest);

                DatabaseManager database = new DatabaseManager(UserHelper.GetClaims(User).GetUserId());

                await database.SetExerciseCompleted(exerciseId);

                if (body != null && body.Valid)
                {
                    List<Difficulty> difficultyList = new List<Difficulty>();
                    if (body.IncludeEasy) difficultyList.Add(Difficulty.Easy);
                    if (body.IncludeMedium) difficultyList.Add(Difficulty.Medium);
                    if (body.IncludeHard) difficultyList.Add(Difficulty.Hard);

                    return new ApiResponse(await database.GetExerciseForUserAsync(difficultyList.ToArray(), body.CourseId));
                }

                return new ApiResponse();
            }
            catch (ApiException exception)
            {
                return new ApiResponse(exception);
            }
        }

        [Authorize]
        [HttpPost("exercise/setNotCompleted")]
        public async Task<IActionResult> UnCompleteExercise(int exerciseId)
        {
            try
            {
                if (exerciseId == 0)
                    return new ApiResponse("Missing exerciseId query parameter, should be id of exercise", HttpStatusCode.BadRequest);

                DatabaseManager database = new DatabaseManager(UserHelper.GetClaims(User).GetUserId());

                await database.SetExerciseNotCompleted(exerciseId);

                return new ApiResponse();
            }
            catch (ApiException exception)
            {
                return new ApiResponse(exception);
            }
        }
    }
}
