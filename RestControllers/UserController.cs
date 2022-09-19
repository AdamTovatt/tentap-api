using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sakur.WebApiUtilities.Models;
using System.Net;
using System.Threading.Tasks;
using TentaPApi.Helpers;
using TentaPApi.Managers;
using TentaPApi.Models;
using TentaPApi.RequestBodies;
using WebApiUtilities.Helpers;

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
            try
            {
                DatabaseManager database = new DatabaseManager();

                if (!requestBody.Valid)
                    return new ApiResponse(requestBody.GetInvalidBodyMessage(), HttpStatusCode.BadRequest);

                User user = requestBody.GetUser();
                user.Password = PasswordHelper.CreatePasswordHash(user.Password);

                user = await database.AddUser(user);

                user = await database.GetUserByEmailAsync(requestBody.Email);

                UserLoginRequestBody authenticatedUser = UserHelper.AuthenticateUser(new UserLoginRequestBody() {Email = requestBody.Email, Password = requestBody.Password }, user);

                if (authenticatedUser != null)
                {
                    var tokenString = UserHelper.GenerateJsonWebToken(user);
                    return new ApiResponse(new TokenResponse(tokenString, user) { Role = user.Role });
                }

                return new ApiResponse(user, HttpStatusCode.InternalServerError);
            }
            catch (ApiException apiException)
            {
                return new ApiResponse(apiException);
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> GetInformation([FromBody] UserLoginRequestBody userLoginInformation)
        {
            try
            {
                if(!userLoginInformation.Valid)
                    return new ApiResponse(userLoginInformation.GetInvalidBodyMessage(), HttpStatusCode.BadRequest);

                DatabaseManager database = new DatabaseManager();
                User user = await database.GetUserByEmailAsync(userLoginInformation.Email);

                if(user == null)
                    return new ApiResponse("Invalid username and/or password", HttpStatusCode.Unauthorized);

                UserLoginRequestBody authenticatedUser = UserHelper.AuthenticateUser(userLoginInformation, user);

                if (authenticatedUser != null)
                {
                    var tokenString = UserHelper.GenerateJsonWebToken(user);
                    return new ApiResponse(new TokenResponse(tokenString, user) { Role = user.Role });
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
