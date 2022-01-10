using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TentaPApi.Data;
using TentaPApi.Helpers;
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

        public async Task<Course> GetCourse(int id)
        {
            string query = @"SELECT
                            c.""Id"" ""CourseId"",
                            c.""Name"" ""CourseName"",
                            m.""Id"" ""ModuleId"",
                            m.""Name"" ""ModuleName"",
                            t.""Id"" ""TagId"",
                            t.""Name"" ""TagName"",
                            COALESCE(q.""Exercises"", 0) as ""ExerciseCount""
                            FROM ""Course"" c
                            JOIN ""Module"" m on m.""CourseId"" = c.""Id""
                            LEFT JOIN ""Tag"" t on t.""ModuleId"" = m.""Id""
                            LEFT JOIN
                            (SELECT

                                ""TagId"",
	                            COUNT(""Id"") ""Exercises""
                            FROM ""Exercise""
                            GROUP BY ""TagId"") q on q.""TagId"" = t.""Id""
                            WHERE c.""Id"" = @id
                            ORDER BY m.""Id"", t.""Id""";

            Course result = null;
            Dictionary<int, Module> modules = new Dictionary<int, Module>();

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                await connection.OpenAsync();

                command.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer).Value = id;

                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        if (result == null)
                        {
                            result = new Course() { Id = (int)reader["CourseId"], Name = reader["CourseName"] as string, Modules = new List<Module>() };
                        }

                        int moduleId = (int)reader["ModuleId"];
                        Module module = null;

                        if (modules.ContainsKey(moduleId))
                        {
                            module = modules[moduleId];
                        }
                        else
                        {
                            module = new Module()
                            {
                                Name = reader["ModuleName"] as string,
                                Id = moduleId,
                                Tags = new List<Tag>()
                            };

                            modules.Add(moduleId, module);
                        }

                        if (reader["TagId"].GetType() != typeof(DBNull))
                        {
                            Tag tag = new Tag((long)reader["ExerciseCount"])
                            {
                                Name = reader["TagName"] as string,
                                Id = (int)reader["TagId"]
                            };

                            module.Tags.Add(tag);
                        }
                    }
                }
            }

            result.Modules = modules.Values.ToList();

            return result;
        }
    }
}
