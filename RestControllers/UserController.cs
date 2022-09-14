﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sakur.WebApiUtilities.Models;
using System.Net;
using System.Threading.Tasks;
using TentaPApi.Helpers;
using TentaPApi.Managers;
using TentaPApi.Models;
using TentaPApi.RequestBodies;

namespace TentaPApi.RestControllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateUserRequestBody requestBody)
        {
            DatabaseManager database = new DatabaseManager();

            if (!requestBody.Valid)
                return new ApiResponse(requestBody.GetInvalidBodyMessage(), HttpStatusCode.BadRequest);

            User createdUser = await database.AddUser(requestBody.GetUser());
            return new ApiResponse(createdUser);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> GetInformation([FromBody] UserLoginRequestBody userLoginInformation)
        {
            try
            {
                DatabaseManager database = new DatabaseManager();
                User user = await database.GetUserByEmailAsync(userLoginInformation.Email);

                UserLoginRequestBody authenticatedUser = UserHelper.AuthenticateUser(userLoginInformation, user);

                if (authenticatedUser != null)
                {
                    var tokenString = UserHelper.GenerateJsonWebToken(user);
                    return new ApiResponse(new TokenResponse(tokenString, user));
                }

                return new ApiResponse("Invalid username and/or password", HttpStatusCode.Unauthorized);
            }
            catch (ApiException apiException)
            {
                return new ApiResponse(apiException);
            }
        }
    }
}
