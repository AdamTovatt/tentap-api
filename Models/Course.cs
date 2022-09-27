using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TentaPApi.Data
{
    public class Course
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        public Course() { }

        public Course(int id, string name, string code)
        {
            Id = id;
            Name = name;
            Code = code;
        }

        public static Course FromReader(NpgsqlDataReader reader)
        {
            return new Course((int)reader["id"], reader["name"] as string, reader["code"] as string) { Active = (bool)reader["active"] };
        }
    }
}
