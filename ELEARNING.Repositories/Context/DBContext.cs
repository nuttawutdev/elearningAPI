using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ELEARNING.Repositories.Context
{
    public class DBContext
    {
        private readonly IConfiguration _configuration;
        public DBContext(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentException(nameof(configuration));
        }

        public IDbConnection CreateConnection() => new SqlConnection(_configuration.GetConnectionString("APPDBCONNECT"));
    }
}