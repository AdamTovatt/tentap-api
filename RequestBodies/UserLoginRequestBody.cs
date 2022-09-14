using Newtonsoft.Json;
using Sakur.WebApiUtilities.BaseClasses;

namespace TentaPApi.RequestBodies
{
    public class UserLoginRequestBody : RequestBody
    {
        [JsonProperty("email")]
        public string RawEmail { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        public string Email { get { return RawEmail.ToLower(); } set { RawEmail = value; } }

        public override bool Valid
        {
            get
            {
                return !string.IsNullOrEmpty(RawEmail) && !string.IsNullOrEmpty(Password);
            }
        }

        public UserLoginRequestBody() { }
    }
}
