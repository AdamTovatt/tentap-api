using Newtonsoft.Json;
using Sakur.WebApiUtilities.BaseClasses;

namespace TentaPApi.RequestBodies
{
    public class UserLoginRequestBody : RequestBody
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        public override bool Valid
        {
            get
            {
                return !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password);
            }
        }

        public UserLoginRequestBody() { }
    }
}
