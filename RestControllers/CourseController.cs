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
        public async Task<IActionResult> GetCourses()
        {
            try
            {
                DatabaseManager database = new DatabaseManager(UserHelper.GetClaims(User).GetUserId());

                return new ApiResponse(await database.GetCoursesAsync());
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
    }
}
