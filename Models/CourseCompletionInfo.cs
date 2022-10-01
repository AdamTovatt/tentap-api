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

            while(await reader.ReadAsync())
            {
                completionInfos.Add((int)reader["difficulty"], new DifficultyCompletionInfo() { Completed = (int)(long)reader["completed_count"], Total = (int)(long)reader["total_count"] });
            }

            if (completionInfos.ContainsKey(1) && completionInfos.ContainsKey(2) && completionInfos.ContainsKey(3))
            {
                return new CourseCompletionInfo()
                {
                    Easy = completionInfos[1],
                    Medium = completionInfos[2],
                    Hard = completionInfos[3],
                };
            }

            throw new ApiException("Error when getting course completion info", System.Net.HttpStatusCode.InternalServerError);
        }
    }
}
