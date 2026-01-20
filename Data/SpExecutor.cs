using Dapper;
using System.Data;

namespace TriviaGame.Api.Data
{
    /// <summary>
    /// Clase responsable de centralizar la ejecución de Stored Procedures
    /// con los paremetros de entrada y salida estandarizados (lo del SpExecutor.md).
    /// 
    /// </summary>
    public class SpExecutor
    {
        private readonly DapperContext _context;

        public SpExecutor(DapperContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Ejecuta un SP que devuelve un flag (bool) y mensaje (string) como parámetros OUTPUT.
        /// </summary>
        /// <param name="spName">Nombre del SP</param>
        /// <param name="parameters">Parámetros de entrada/salida</param>
        /// <returns>Tuple con flag (true/false) y mensaje</returns>
        public async Task<(bool Flag, string Message)> ExecuteWithOutputAsync(string spName, DynamicParameters parameters)
        {
            using var connection = _context.CreateConnection();

            // Ejecuta el SP
            await connection.ExecuteAsync(spName, parameters, commandType: CommandType.StoredProcedure);

            // Obtiene el valor de @Success o @Exists si existe
            bool flag = false;
            if (parameters.ParameterNames.Contains("@Success"))
                flag = parameters.Get<bool>("@Success");
            else if (parameters.ParameterNames.Contains("@Exists"))
                flag = parameters.Get<bool>("@Exists");

            // Obtiene el mensaje de salida
            string message = parameters.Get<string>("@ResponseMessage");

            return (flag, message);
        }


        /// <summary>
        /// Genera un objeto DynamicParameters estandar para SPs que devuelven @Success, @Exists y @ResponseMessage
        /// La clave (Key) es el nombre del parámetro del SP, por ejemplo "@Gmail".
        // El valor (Value) es el valor que quieres pasar al SP, por ejemplo "correo@gmail.com".
        /// <returns>DynamicParameters listo para ejecutar SP</returns>
        /// <param name="inputs">Diccionario con parametros de entrada</param>
        /// </summary>
        public DynamicParameters CreateOutputParameters(Dictionary<string, object> inputs)
        {
            var parameters = new DynamicParameters();

            // Agrega inputs
            //key value pair = kvp
            foreach (var kvp in inputs)
                parameters.Add(kvp.Key, kvp.Value);

            // Agrega outputs estándar
            parameters.Add("@Success", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@Exists", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@ResponseMessage", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

            return parameters;
        }

        /// <summary>
        /// Ejecuta un SP que devuelve un solo resultado mapeado a T
        /// </summary>
        /// <typeparam name="T">Tipo del objeto de resultado (un model )</typeparam>
        /// <param name="spName">Nombre del SP</param>
        /// <param name="parameters">parametros de entrada</param>
        /// <returns>Objeto de tipo T o null si no hay resultado</returns>
        public async Task<T?> QuerySingleAsync<T>(string spName, object? parameters = null)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<T>(spName, parameters, commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Ejecuta un SP que devuelve multiples resultados mapeados a T (puede ser cualquier clase)
        /// </typeparam>
        /// <param name="spName">Nombre del SP</param>
        /// <param name="parameters">parametros de entrada</param>
        /// <returns>IEnumerable de T</returns>
        /// <typeparam name="T">Tipo del objeto de resultado (un model o cualquier clase)</typeparam>
        /// 
        /// </summary>
        public async Task<IEnumerable<T>> QueryAsync<T>(string spName, object? parameters = null)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<T>(
                spName,
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public DynamicParameters CreateInputParameters(Dictionary<string, object> inputs)
        {
            var parameters = new DynamicParameters();
            foreach (var kvp in inputs)
                parameters.Add(kvp.Key, kvp.Value);

            return parameters;
        }

    }
}
