using Newtonsoft.Json;
using Sakur.WebApiUtilities.BaseClasses;
using System;
using TentaPApi.Models;

namespace TentaPApi.RequestBodies
{
    public class CreateUserRequestBody : RequestBody
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        public override bool Valid
        {
            get
            {
                return !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(Email);
            }
        }

        public CreateUserRequestBody() { }

        public User GetUser()
        {
            return new User(Name, Email, Password, DateTime.Now, 0);
        }
    }
}
