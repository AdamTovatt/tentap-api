using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TentaPApi.Data
{
    public class Module
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Module() { }

        public Module(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public static Module FromReader(NpgsqlDataReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
