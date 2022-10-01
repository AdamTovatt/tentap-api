using Newtonsoft.Json;
using Npgsql;
using Sakur.WebApiUtilities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TentaPApi.Models
{
    public class CourseCompletionInfo
    {
        [JsonProperty("easy")]
        public DifficultyCompletionInfo Easy { get; set; }

        [JsonProperty("medium")]
        public DifficultyCompletionInfo Medium { get; set; }

        [JsonProperty("hard")]
        public DifficultyCompletionInfo Hard { get; set; }

        public static async Task<CourseCompletionInfo> FromReaderAsync(NpgsqlDataReader reader)
        {
            Dictionary<int, DifficultyCompletionInfo> completionInfos = new Dictionary<int, DifficultyCompletionInfo>();

            while (await reader.ReadAsync())
            {
                completionInfos.Add((int)reader["difficulty"], new DifficultyCompletionInfo() { Completed = (int)(long)reader["completed_count"], Total = (int)(long)reader["total_count"] });
            }

            for (int i = 1; i < 4; i++)
            {
                if (!completionInfos.ContainsKey(i))
                    completionInfos.Add(i, new DifficultyCompletionInfo());
            }

            return new CourseCompletionInfo()
            {
                Easy = completionInfos[1],
                Medium = completionInfos[2],
                Hard = completionInfos[3],
            };
        }
    }
}
