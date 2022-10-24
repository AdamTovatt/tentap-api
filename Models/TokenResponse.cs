using Newtonsoft.Json;
using System;

namespace TentaPApi.Models
{
    public class TokenResponse
    {
        [JsonProperty("token")]
        public string Token { get; private set; }

        [JsonProperty("tokenExpirationDate")]
        public DateTime TokenExpirationDate { get; set; }

        [JsonProperty("userId")]
        public int UserId { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; }

        public TokenResponse(string token, User user, DateTime expirationDate)
        {
            Token = token;
            TokenExpirationDate = expirationDate;
            UserId = user.Id;
            Email = user.Email;
            Name = user.Name;
            Role = user.Role;
        }
    }
}
