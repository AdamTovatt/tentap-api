using Newtonsoft.Json;

namespace TentaPApi.Models
{
    public class TokenResponse
    {
        [JsonProperty("token")]
        public string Token { get; private set; }

        [JsonProperty("userId")]
        public int UserId { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        public TokenResponse(string token, User user)
        {
            Token = token;
            UserId = user.Id;
            Email = user.Email;
            Name = user.Name;
        }
    }
}
