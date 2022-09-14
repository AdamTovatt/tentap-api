using Npgsql;
using NpgsqlTypes;
using Sakur.WebApiUtilities.Models;
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

        public async Task<User> GetUserByEmailAsync(string email)
        {
            User result = null;

            const string query = "SELECT id, name, email, password, created_date FROM site_user WHERE email = @email";

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                await connection.OpenAsync();

                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        result = User.FromReader(reader);
                        return result;
                    }
                }
            }

            return result;
        }

        public async Task<User> AddUser(User user)
        {
            const string query = @"INSERT INTO site_user (name, email, password, created_date)
                                    VALUES (@name, @email, @password, NOW())
                                    RETURNING id";

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                await connection.OpenAsync();

                command.Parameters.Add("@name", NpgsqlDbType.Integer).Value = user.Name;
                command.Parameters.Add("@email", NpgsqlDbType.Integer).Value = user.Email.ToLower();
                command.Parameters.Add("@password", NpgsqlDbType.Integer).Value = user.Password;

                user.Id = (int)await command.ExecuteScalarAsync();

                return user;
            }
        }

        public async Task<Exercise> AddExerciseAsync(Exercise exercise)
        {
            const string query = @"INSERT INTO exercise (module_id, difficulty, source_id, problem_image, solution_image)
                                    VALUES (@module_id, @difficulty, @source_id, @problem_image, @solution_image)
                                    RETURNING id";

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                await connection.OpenAsync();

                command.Parameters.Add("@module_id", NpgsqlDbType.Integer).Value = exercise.Module.Id;
                command.Parameters.Add("@difficulty", NpgsqlDbType.Integer).Value = exercise.Difficulty;
                command.Parameters.Add("@source_id", NpgsqlDbType.Integer).Value = exercise.Source.Id;
                command.Parameters.Add("@problem_image", NpgsqlDbType.Varchar).Value = exercise.ProblemImage.Url;
                command.Parameters.Add("@solution_image", NpgsqlDbType.Varchar).Value = exercise.SolutionImage.Url;

                exercise.Id = (int)await command.ExecuteScalarAsync();

                return exercise;
            }
        }

        public async Task<Source> AddSourceAsync(Source source)
        {
            const string query = "INSERT INTO source (course_id, author, date) VALUES (@courseId, @author, @date) RETURNING id";

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                await connection.OpenAsync();

                command.Parameters.Add("@courseId", NpgsqlDbType.Integer).Value = source.Course.Id;
                command.Parameters.Add("@author", NpgsqlDbType.Varchar).Value = source.Author;
                command.Parameters.Add("@date", NpgsqlDbType.Date).Value = source.Date;

                source.Id = (int)await command.ExecuteScalarAsync();

                return source;
            }
        }

        public async Task<Module> AddModuleAsync(Module module)
        {
            const string query = "INSERT INTO course_module (name, courseId) VALUES (@name, @courseId) RETURNING id";

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                await connection.OpenAsync();

                command.Parameters.Add("@courseId", NpgsqlDbType.Integer).Value = module.Course.Id;
                command.Parameters.Add("@name", NpgsqlDbType.Varchar).Value = module.Name;

                module.Id = (int)await command.ExecuteScalarAsync();

                return module;
            }
        }

        public async Task<Course> AddCourseAsync(Course course)
        {
            const string query = "INSERT INTO course (code, name) VALUES (@code, @name)";

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                await connection.OpenAsync();

                command.Parameters.Add("@code", NpgsqlDbType.Varchar).Value = course.Code;
                command.Parameters.Add("@name", NpgsqlDbType.Varchar).Value = course.Name;

                course.Id = (int)await command.ExecuteScalarAsync();

                return course;
            }
        }

        public async Task<List<Course>> GetCoursesAsync()
        {
            List<Course> result = new List<Course>();

            const string query = "SELECT id, code, name FROM course";

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                await connection.OpenAsync();

                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        result.Add(Course.FromReader(reader));
                    }
                }
            }

            return result;
        }

        public async Task<Course> GetCourseAsync(int id)
        {
            const string query = "SELECT id, code, name FROM course WHERE id = @id";

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

        public async Task<Source> GetSourceAsync(int id)
        {
            const string query = "SELECT id, course_id, author, source_date, created_by FROM source WHERE id = @id";

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                await connection.OpenAsync();

                command.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;

                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        return Source.FromReader(reader);
                    }
                }
            }

            return null;
        }

        public async Task<Module> GetModuleAsync(int id)
        {
            const string query = "SELECT id, course_id, name, created_by FROM module WHERE id = @id";

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                await connection.OpenAsync();

                command.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;

                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        return Module.FromReader(reader);
                    }
                }
            }

            return null;
        }

        public async Task<Exercise> GetExerciseByIdAsync(int id)
        {
            const string query = @"SELECT
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
                                    WHERE id = @id";

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                await connection.OpenAsync();

                command.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;

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

        public async Task<bool> RemoveExerciseAsync(int id)
        {
            const string query = "DELETE FROM exercise WHERE id = @id";

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                await connection.OpenAsync();

                command.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;

                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<Exercise> GetExerciseForUserAsync(int userId, Difficulty[] difficulties)
        {
            List<Exercise> exercises = new List<Exercise>();

            const string query = @"SELECT
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
                                    TOP 10";

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
                        exercises.Add(Exercise.FromReader(reader));
                    }
                }
            }

            if (exercises.Count == 0)
                return null;

            return exercises.TakeRandom();
        }
    }
}
