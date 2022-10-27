using Npgsql;
using NpgsqlTypes;
using Sakur.WebApiUtilities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        public int UserId { get; set; }

        public DatabaseManager()
        {
            LoadConnectionString();
        }

        public DatabaseManager(int userId)
        {
            LoadConnectionString();
            UserId = userId;
        }

        private void LoadConnectionString()
        {
            ConnectionString = ConnectionStringHelper.GetConnectionStringFromUrl(EnvironmentHelper.GetEnvironmentVariable("DATABASE_URL"));
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            User result = null;

            const string query = "SELECT id, name, email, password, created_date, role FROM site_user WHERE email = @email";

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                await connection.OpenAsync();

                command.Parameters.Add("@email", NpgsqlDbType.Varchar).Value = email.ToLower();

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
            try
            {
                const string query = @"INSERT INTO site_user (name, email, password, created_date)
                                    VALUES (@name, @email, @password, NOW())
                                    RETURNING id";

                using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    await connection.OpenAsync();

                    command.Parameters.Add("@name", NpgsqlDbType.Varchar).Value = user.Name;
                    command.Parameters.Add("@email", NpgsqlDbType.Varchar).Value = user.Email.ToLower();
                    command.Parameters.Add("@password", NpgsqlDbType.Varchar).Value = user.Password;

                    user.Id = (int)await command.ExecuteScalarAsync();

                    return user;
                }
            }
            catch (PostgresException postgresException)
            {
                if (postgresException.SqlState == "23505") //unique constraint violation
                    throw new ApiException("Email already registered", System.Net.HttpStatusCode.BadRequest);
                else
                    throw new ApiException(postgresException.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<Exercise> UpdateExerciseAsync(Exercise exercise)
        {
            const string query = @"UPDATE exercise SET
                                    module_id = @module_id,
                                    difficulty = @difficulty,
                                    source_id = @source_id,
                                    problem_image = @problem_image,
                                    solution_image = @solution_image
                                    WHERE id = @id";

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                await connection.OpenAsync();

                command.Parameters.Add("@module_id", NpgsqlDbType.Integer).Value = exercise.Module.Id;
                command.Parameters.Add("@difficulty", NpgsqlDbType.Integer).Value = (int)exercise.Difficulty;
                command.Parameters.Add("@source_id", NpgsqlDbType.Integer).Value = exercise.Source.Id;
                command.Parameters.Add("@problem_image", NpgsqlDbType.Varchar).Value = GetImageParameterValue(exercise.ProblemImage);
                command.Parameters.Add("@solution_image", NpgsqlDbType.Varchar).Value = GetImageParameterValue(exercise.SolutionImage);
                command.Parameters.Add("@id", NpgsqlDbType.Integer).Value = exercise.Id;

                await command.ExecuteNonQueryAsync();

                return exercise;
            }
        }

        public async Task<Exercise> AddExerciseAsync(Exercise exercise)
        {
            const string query = @"INSERT INTO exercise (module_id, difficulty, source_id, problem_image, solution_image, created_by)
                                    VALUES (@module_id, @difficulty, @source_id, @problem_image, @solution_image, @created_by)
                                    RETURNING id";

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                await connection.OpenAsync();

                command.Parameters.Add("@module_id", NpgsqlDbType.Integer).Value = exercise.Module.Id;
                command.Parameters.Add("@difficulty", NpgsqlDbType.Integer).Value = (int)exercise.Difficulty;
                command.Parameters.Add("@source_id", NpgsqlDbType.Integer).Value = exercise.Source.Id;
                command.Parameters.Add("@problem_image", NpgsqlDbType.Varchar).Value = GetImageParameterValue(exercise.ProblemImage);
                command.Parameters.Add("@solution_image", NpgsqlDbType.Varchar).Value = GetImageParameterValue(exercise.SolutionImage);
                command.Parameters.Add("@created_by", NpgsqlDbType.Integer).Value = UserId;

                exercise.Id = (int)await command.ExecuteScalarAsync();

                return exercise;
            }
        }

        private object GetImageParameterValue(CloudinaryImage image)
        {
            if (image == null || image.Url == null)
                return DBNull.Value;

            return image.Url;
        }

        public async Task<Source> AddSourceAsync(Source source)
        {
            const string query = "INSERT INTO source (course_id, author, source_date, created_by) VALUES (@courseId, @author, @date, @created_by) RETURNING id";

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                await connection.OpenAsync();

                command.Parameters.Add("@courseId", NpgsqlDbType.Integer).Value = source.Course.Id;
                command.Parameters.Add("@author", NpgsqlDbType.Varchar).Value = source.Author;
                command.Parameters.Add("@date", NpgsqlDbType.Date).Value = source.Date;
                command.Parameters.Add("@created_by", NpgsqlDbType.Integer).Value = UserId;

                source.Id = (int)await command.ExecuteScalarAsync();

                return source;
            }
        }

        public async Task<Module> AddModuleAsync(Module module)
        {
            const string query = "INSERT INTO course_module (name, course_id, created_by) VALUES (@name, @courseId, @created_by) RETURNING id";

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                await connection.OpenAsync();

                command.Parameters.Add("@courseId", NpgsqlDbType.Integer).Value = module.Course.Id;
                command.Parameters.Add("@name", NpgsqlDbType.Varchar).Value = module.Name;
                command.Parameters.Add("@created_by", NpgsqlDbType.Integer).Value = UserId;

                module.Id = (int)await command.ExecuteScalarAsync();

                return module;
            }
        }

        public async Task<Course> AddCourseAsync(Course course)
        {
            const string query = "INSERT INTO course (code, name, created_by) VALUES (@code, @name, @created_by) RETURNING id";

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                await connection.OpenAsync();

                command.Parameters.Add("@code", NpgsqlDbType.Varchar).Value = course.Code;
                command.Parameters.Add("@name", NpgsqlDbType.Varchar).Value = course.Name;
                command.Parameters.Add("@created_by", NpgsqlDbType.Integer).Value = UserId;

                course.Id = (int)await command.ExecuteScalarAsync();

                return course;
            }
        }

        public async Task<List<Course>> GetCoursesAsync(bool includeInactive)
        {
            List<Course> result = new List<Course>();

            string query = "SELECT id, code, name, active FROM course WHERE active = true";
            if(includeInactive)
                query = "SELECT id, code, name, active FROM course";

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

        public async Task SetExerciseCompleted(int exerciseId, int perceivedDifficulty)
        {
            if (UserId == 0)
                throw new ApiException("Missing user information when setting exercise completed", System.Net.HttpStatusCode.BadRequest);

            const string query = @"INSERT INTO user_completed_exercise (user_id, exercise_id, perceived_difficulty, completed_time)
                                   VALUES (@user_id, @exercise_id, @perceived_difficulty, NOW())
                                   ON CONFLICT(user_id, exercise_id) DO
                                   UPDATE SET perceived_difficulty = @perceived_difficulty, completed_time = NOW()
                                   WHERE user_completed_exercise.user_id = @user_id AND user_completed_exercise.exercise_id = @exercise_id";

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                await connection.OpenAsync();

                command.Parameters.Add("@user_id", NpgsqlDbType.Integer).Value = UserId;
                command.Parameters.Add("@exercise_id", NpgsqlDbType.Integer).Value = exerciseId;
                command.Parameters.Add("@perceived_difficulty", NpgsqlDbType.Integer).Value = perceivedDifficulty;

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task SetExerciseNotCompleted(int exerciseId)
        {
            if (UserId == 0)
                throw new ApiException("Missing user information when setting exercise completed", System.Net.HttpStatusCode.BadRequest);

            const string query = "DELETE FROM user_completed_exercise WHERE user_id = @user_id AND exercise_id = @exercise_id";

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                await connection.OpenAsync();

                command.Parameters.Add("@user_id", NpgsqlDbType.Integer).Value = UserId;
                command.Parameters.Add("@exercise_id", NpgsqlDbType.Integer).Value = exerciseId;

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<Course> GetCourseAsync(int id)
        {
            const string query = "SELECT id, code, active, name FROM course WHERE id = @id";

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

        public async Task<List<Source>> GetSourcesByCourseAsync(int courseId)
        {
            const string query = "SELECT id, course_id, author, source_date, created_by FROM source WHERE course_id = @id";

            List<Source> result = new List<Source>();

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                await connection.OpenAsync();

                command.Parameters.Add("@id", NpgsqlDbType.Integer).Value = courseId;

                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        result.Add(Source.FromReader(reader));
                    }
                }
            }

            return result;
        }

        public async Task<List<Module>> GetModulesByCourseAsync(int courseId)
        {
            const string query = "SELECT id, course_id, name, created_by FROM course_module WHERE course_id = @id";

            List<Module> result = new List<Module>();

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                await connection.OpenAsync();

                command.Parameters.Add("@id", NpgsqlDbType.Integer).Value = courseId;

                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        result.Add(Module.FromReader(reader));
                    }
                }
            }

            return result;
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
            const string query = "SELECT id, course_id, name, created_by FROM course_module WHERE id = @id";

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
	                                    CASE WHEN uce.exercise_id IS NULL THEN false ELSE true END AS completed,
                                        e.active,
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
                                    JOIN
                                        course_module m ON e.module_id = m.id
                                    LEFT JOIN
                                    	user_completed_exercise uce ON uce.user_id = @user_id AND uce.exercise_id = @id
                                    WHERE e.id = @id";

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                await connection.OpenAsync();

                command.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;
                command.Parameters.Add("@user_id", NpgsqlDbType.Integer).Value = UserId;

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

        public async Task<bool> DeActivateCourseAsync(int courseId)
        {
            const string query = "UPDATE course SET active = false WHERE id = @id";

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                await connection.OpenAsync();

                command.Parameters.Add("@id", NpgsqlDbType.Integer).Value = courseId;

                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> DeActivateExerciseAsync(int exerciseId)
        {
            const string query = "UPDATE exercise SET active = false WHERE id = @id";

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                await connection.OpenAsync();

                command.Parameters.Add("@id", NpgsqlDbType.Integer).Value = exerciseId;

                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> SetCourseActiveStatusAsync(int courseId, bool active)
        {
            const string query = "UPDATE course SET active = @active WHERE id = @id";

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                await connection.OpenAsync();

                command.Parameters.Add("@id", NpgsqlDbType.Integer).Value = courseId;
                command.Parameters.Add("@active", NpgsqlDbType.Boolean).Value = active;

                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> SetExerciseActiveStatus(int exerciseId, bool active)
        {
            const string query = "UPDATE exercise SET active = @active WHERE id = @id";

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                await connection.OpenAsync();

                command.Parameters.Add("@id", NpgsqlDbType.Integer).Value = exerciseId;
                command.Parameters.Add("@active", NpgsqlDbType.Boolean).Value = active;

                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<List<Exercise>> GetExercisesByCourseAsync(int courseId, bool onlyInactive)
        {
            List<Exercise> exercises = new List<Exercise>();

            string query = @"SELECT
	                                    e.id,
                                        false as completed,
                                        e.active,
	                                    e.module_id,
	                                    e.problem_image,
	                                    e.difficulty,
	                                    e.solution_image,
	                                    e.source_id,
                                        e.created_date,
	                                    s.author,
	                                    s.course_id,
	                                    s.source_date,
	                                    m.name,
	                                    m.id ""module_id""
                                    FROM
                                        exercise e
                                    JOIN
                                        source s ON e.source_id = s.id
                                    JOIN
                                        course_module m ON e.module_id = m.id
                                    WHERE
                                        m.course_id = @course_id";

            if (onlyInactive)
            {
                query += @" AND e.active = false ORDER BY e.created_date DESC";
            }
            else
            {
                query += @" ORDER BY e.created_date DESC";
            }

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                await connection.OpenAsync();

                command.Parameters.Add("@course_id", NpgsqlDbType.Integer).Value = courseId;

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

            return exercises;
        }

        public async Task<CourseCompletionInfo> GetCourseCompletionInfoAsync(int courseId)
        {
            if(UserId == 0)
                throw new ApiException("Can not get completion info without user information, a bearer token needs to be provided", HttpStatusCode.BadRequest);

            const string query = @"SELECT
	                                    q1.difficulty,
	                                    CASE WHEN q2.completed_count IS NULL THEN 0 ELSE q2.completed_count END,
	                                    q1.total_count
                                    FROM
                                    (SELECT
	                                    e.difficulty,
	                                    COUNT(e.id) AS total_count
                                    FROM
	                                    exercise e
                                    JOIN
	                                    source s ON e.source_id = s.id
                                    WHERE
	                                    s.course_id = @course_id
                                    GROUP BY
	                                    e.difficulty) q1
                                    LEFT JOIN
                                    (SELECT
	                                    e.difficulty,
	                                    COUNT(e.id) AS completed_count
                                    FROM
	                                    exercise e
                                    JOIN
	                                    source s ON e.source_id = s.id
                                    LEFT JOIN
	                                    user_completed_exercise uce on e.id = uce.exercise_id
                                    WHERE
	                                    s.course_id = @course_id AND
	                                    uce.user_id = @user_id
                                    GROUP BY
	                                    e.difficulty) q2
                                    ON q1.difficulty = q2.difficulty";

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                await connection.OpenAsync();

                command.Parameters.Add("@user_id", NpgsqlDbType.Integer).Value = UserId;
                command.Parameters.Add("@course_id", NpgsqlDbType.Integer).Value = courseId;

                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    return await CourseCompletionInfo.FromReaderAsync(reader);
                }
            }
        }

        public async Task<Exercise> GetExerciseForUserAsync(Difficulty[] difficulties, int courseId, List<int> moduleExcludeList)
        {
            if (moduleExcludeList == null)
                moduleExcludeList = new List<int>();

            List<Exercise> exercises = new List<Exercise>();

            const string query = @"SELECT
	                                    e.id,
                                        CASE WHEN uce.completed_time IS NULL THEN false ELSE true END completed,
                                        e.active,
	                                    e.module_id,
	                                    e.problem_image,
	                                    e.difficulty,
	                                    e.solution_image,
	                                    e.source_id,
	                                    s.author,
	                                    s.course_id,
	                                    s.source_date,
	                                    m.name,
	                                    uce.completed_time,
                                        uce.perceived_difficulty
                                    FROM
                                        exercise e
                                    JOIN
                                        source s ON e.source_id = s.id
                                    JOIN
                                        course_module m ON e.module_id = m.id
                                    LEFT JOIN
	                                    user_completed_exercise uce ON uce.exercise_id = e.id
                                    WHERE
                                        e.difficulty = ANY(@difficulties) AND
                                        m.course_id = @course_id AND
                                        e.active = TRUE AND
                                        NOT m.id = ANY(@module_exclusion_list)
                                    ORDER BY COALESCE(uce.completed_time, '2000-01-01') ASC, s.source_date DESC
                                    LIMIT 10";

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                await connection.OpenAsync();

                command.Parameters.Add("@user_id", NpgsqlDbType.Integer).Value = UserId;
                command.Parameters.Add("@difficulties", NpgsqlDbType.Array | NpgsqlDbType.Integer).Value = difficulties.ToIntArray();
                command.Parameters.Add("@module_exclusion_list", NpgsqlDbType.Array | NpgsqlDbType.Integer).Value = moduleExcludeList.ToArray();
                command.Parameters.Add("@course_id", NpgsqlDbType.Integer).Value = courseId;

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

            IEnumerable<Exercise> newExercises = exercises.Where(x => !x.Completed);

            if (newExercises.Count() == 0)
                return exercises.TakeRandom();
            else
                return newExercises.TakeRandom();
        }
    }
}
