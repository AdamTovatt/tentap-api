using Newtonsoft.Json;
using Npgsql;
using System;

namespace TentaPApi.Models
{
    public class User
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }

        public User() { }

        public User(string name, string email, string password, DateTime createdDate)
        {
            Name = name;
            Email = email;
            Password = password;
            CreatedDate = createdDate;
        }

        public User(string name, string email, string password, DateTime createdDate, int id)
        {
            Id = id;
            Name = name;
            Email = email;
            Password = password;
            CreatedDate = createdDate;
        }

        public static User FromReader(NpgsqlDataReader reader)
        {
            return new User(reader["name"] as string, reader["email"] as string, reader["password"] as string, (DateTime)reader["created_date"], (int)reader["id"]);
        }
    }
}
