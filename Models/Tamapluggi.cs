using Npgsql;
using System;

namespace TentaPApi.Models
{
    public class Tamapluggi
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int StudyGoal { get; set; }
        public int BreakDuration { get; set; }
        public int UserId { get; set; }

        public static Tamapluggi FromReader(NpgsqlDataReader reader)
        {
            return new Tamapluggi()
            {
                Id = (Guid)reader["id"],
                Name = reader["name"] as string,
                StudyGoal = (int)reader["study_goal"],
                BreakDuration = (int)reader["break_duration"],
                UserId = (int)reader["user_id"]
            };
        }
    }
}
