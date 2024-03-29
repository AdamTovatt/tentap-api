﻿using CloudinaryDotNet;
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

                Source source = body.Id == 0 ? await database.GetSourceAsync(body.SourceId) : null;

                if (source == null && body.Id == 0)
                    return new ApiResponse("Invalid sourceId: " + body.SourceId, HttpStatusCode.BadRequest);

                Module module = body.Id == 0 ? await database.GetModuleAsync(body.ModuleId) : null;

                if (module == null && body.Id == 0)
                    return new ApiResponse("Invalid moduleId: " + body.ModuleId, HttpStatusCode.BadRequest);

                Exercise exercise = body.GetExercise();

                if (body.Id != 0)
                    exercise.Combine(await database.GetExerciseByIdAsync(body.Id));

                if (!string.IsNullOrEmpty(body.ProblemImageData) && !string.IsNullOrEmpty(body.SolutionImageData))
                {
                    ExerciseImageUploader imageUploader = new ExerciseImageUploader(body.ProblemImageData, body.SolutionImageData);
                    bool result = await imageUploader.UploadImagesAsync();

                    if (!result)
                        return new ApiResponse("Error when uploading images", HttpStatusCode.InternalServerError);

                    exercise.ProblemImage = imageUploader.ProblemImage;
                    exercise.SolutionImage = imageUploader.SolutionImage;
                }

                exercise.Source = source == null ? exercise.Source : source;
                exercise.Module = module == null ? exercise.Module : module;

                if (body.Id == 0)
                    exercise = await database.AddExerciseAsync(exercise);
                else
                    exercise = await database.UpdateExerciseAsync(exercise);

                return new ApiResponse(exercise);
            }
            catch (ApiException exception)
            {
                return new ApiResponse(exception);
            }
        }

        [Authorize(Roles = "1,2")]
        [HttpPost("course/setActive")]
        public async Task<IActionResult> SetCourseActiveStatus(int id, bool active)
        {
            try
            {
                DatabaseManager database = new DatabaseManager(UserHelper.GetClaims(User).GetUserId());
                Course course = await database.GetCourseAsync(id);

                if (course == null)
                    return new ApiResponse("No such course, id: " + id, HttpStatusCode.BadRequest);

                if (!await database.SetCourseActiveStatusAsync(id, active))
                    return new ApiResponse("Error when activating course: " + id, HttpStatusCode.InternalServerError);

                return new ApiResponse("Exercise was activated");
            }
            catch (ApiException exception)
            {
                return new ApiResponse(exception);
            }
        }


        [Authorize(Roles = "1,2")]
        [HttpPost("exercise/setActive")]
        public async Task<IActionResult> SetExerciseActiveStatus(int id, bool active)
        {
            try
            {
                DatabaseManager database = new DatabaseManager(UserHelper.GetClaims(User).GetUserId());
                Exercise exercise = await database.GetExerciseByIdAsync(id);

                if (exercise == null)
                    return new ApiResponse("No such exercise, id: " + id, HttpStatusCode.BadRequest);

                if (!await database.SetExerciseActiveStatus(id, active))
                    return new ApiResponse("Error when activating exercise: " + id, HttpStatusCode.InternalServerError);

                return new ApiResponse("Exercise was activated");
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
