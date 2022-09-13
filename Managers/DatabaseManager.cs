using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TentaPApi.Data;
using TentaPApi.Helpers;
using TentaPApi.Models;
using WebApiUtilities.Helpers;

namespace TentaPApi.Managers
{
    public class DatabaseManager
    {
        public string ConnectionString { get; private set; }

        public DatabaseManager()
        {
            ConnectionString = ConnectionStringHelper.GetConnectionStringFromUrl(EnvironmentHelper.GetEnvironmentVariable("DATABASE_URL"));
        }

        public async Task<bool> AddCourseAsync(string code, string name)
        {
            string query = "INSERT INTO course (code, name) VALUES (@code, @name)";

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                await connection.OpenAsync();

                command.Parameters.Add("@code", NpgsqlDbType.Varchar).Value = code;
                command.Parameters.Add("@name", NpgsqlDbType.Varchar).Value = name;

                int queryResult = await command.ExecuteNonQueryAsync();

                return queryResult == 1 || queryResult == -1;
            }
        }

        public async Task<List<Course>> GetCoursesAsync()
        {
            List<Course> result = new List<Course>();

            string query = "SELECT id, code, name FROM course";

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                await connection.OpenAsync();

                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while(await reader.ReadAsync())
                    {
                        result.Add(Course.FromReader(reader));
                    }
                }
            }

            return result;
        }

        public async Task<Course> GetCourseAsync(int id)
        {
            string query = "SELECT id, code, name FROM course WHERE id = @id";

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                await connection.OpenAsync();

                command.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;

                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        return Course.FromReader(reader);
                    }
                }
            }

            return null;
        }

        public async Task<Exercise> GetExerciseAsync(int userId, Difficulty[] difficulties)
        {
            //NpgsqlDbType.Array | NpgsqlDbType.Integer

            string query = @"SELECT
	                            e.id,
	                            e.difficulty,
	                            e.module_id,
	                            e.problem_image,
	                            e.solution_image,
	                            e.source_id,
	                            s.author,
	                            s.course_id,
	                            s.source_date,
	                            m.name,
	                            m.id ""module_id""
                            FROM
                                exercise e
                            JOIN
                                source s ON e.source_id = s.id
                                course_module m ON e.module_id = m.id
                            WHERE
                                e.id NOT IN(SELECT id FROM user_completed_exercise uce WHERE uce.user_id = @userId) AND
                                e.difficulty IN @difficulties
                            ORDER BY s.source_date DESC
                            TOP 1";

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                await connection.OpenAsync();

                command.Parameters.Add("@userId", NpgsqlDbType.Integer).Value = userId;
                command.Parameters.Add("@userId", NpgsqlDbType.Array | NpgsqlDbType.Integer).Value = difficulties;

                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        return Exercise.FromReader(reader);
                    }
                }
            }

            return null;
        }
    }
}
