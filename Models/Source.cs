using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TentaPApi.Data
{
    public class Source
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("course")]
        public Course Course { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        public static Source FromReader(NpgsqlDataReader reader)
        {
            return new Source()
            {
                Author = reader["author"] as string,
                Course = new Course() { Id = (int)reader["id"] },
                Date = (DateTime)reader["source_date"],
                Id = (int)reader["id"]
            };
        }
    }
}
