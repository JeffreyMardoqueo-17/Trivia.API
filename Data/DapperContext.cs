using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace TriviaGame.Api.Data
{
    public class DapperContext
    {
        private readonly string _connectionString; //la cadena de conexion

        public DapperContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") // esta es la que agregue en los secretos de usuario
                ?? throw new InvalidOperationException("No se encontró la connection string 'DefaultConnection'.");
        }

        public IDbConnection CreateConnection()
            => new SqlConnection(_connectionString);
    }

}
