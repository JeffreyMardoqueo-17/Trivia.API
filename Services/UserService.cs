using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using TriviaGame.Api.Data;
using TriviaGame.Api.Models;
using TriviaGame.Api.Services.Interfaces;
using TriviaGame.Api.Utils;
using System.Data;
using TriviaGame.Api.Common;


namespace TriviaGame.Api.Services
{
    public class UserService : IUserService
    {
        private readonly SpExecutor _spExecutor;

        public UserService(SpExecutor spExecutor)
        {
            _spExecutor = spExecutor;
        }

        /// <summary>
        /// Crea un usuario usando SP_CreateUser
        /// </summary>
        /// <param name="gmail">Correo del usuario</param>
        /// <param name="password">Contraseña en texto plano</param>
        /// <returns>Tuple indicando si existe y mensaje</returns>
        public async Task<(bool Exists, string Message)> CreateUserAsync(string gmail, string password)
        {
            // 1. Generar salt único
            var salt = PasswordHasher.GenerateSalt();

            // 2. Generar hash de la contraseña con el salt
            var hash = PasswordHasher.HashPassword(password, salt);

            // 3. Preparar parámetros para el SP incluyendo los OUTPUT
            var parameters = new DynamicParameters();
            parameters.Add("@Gmail", gmail, DbType.String, ParameterDirection.Input);
            parameters.Add("@PasswordHash", hash, DbType.String, ParameterDirection.Input);
            parameters.Add("@PasswordSalt", salt, DbType.String, ParameterDirection.Input);
            parameters.Add("@Exists", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@ResponseMessage", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

            // 4. Ejecutar SP
            var result = await _spExecutor.ExecuteWithOutputAsync("SP_CreateUser", parameters);

            return (result.Flag, result.Message);
        }



        /// <summary>
        /// Inicia sesion verificando la contraseña del usuario usando SP y DynamicParameters
        /// </summary>
        /// <param name="gmail">Correo del usuario</param>
        /// <param name="password">Contraseña que ingresa el usuario</param>
        /// <returns>Usuario si la contraseña es correcta, null si falla</returns>

        public async Task<User?> LoginAsync(string gmail, string password)
        {
            var user = await _spExecutor.QuerySingleAsync<User>(
                "SP_LoginUser",
                new { Gmail = gmail }
            );

            if (user == null)
                throw new BusinessException("Usuario o contraseña incorrectos");

            if (!user.IsActive)
                throw new BusinessException("Usuario inactivo");

            PasswordHasher.VerifyPassword(
                password,
                user.PasswordHash,
                user.PasswordSalt
            );

            return user;
        }


        /// <summary>
        /// Actualiza la contraseña del usuario con SP_UpdateUserPassword
        /// </summary>
        public async Task<(bool Success, string Message)> UpdatePasswordAsync(string gmail, string newHash, string newSalt)
        {
            var inputs = new Dictionary<string, object>
            {
                { "@Gmail", gmail },
                { "@NewPasswordHash", newHash },
                { "@NewPasswordSalt", newSalt }
            };

            var parameters = _spExecutor.CreateOutputParameters(inputs);
            var result = await _spExecutor.ExecuteWithOutputAsync("SP_UpdateUserPassword", parameters);
            return (result.Flag, result.Message);
        }

        /// <summary>
        /// Desactiva un usuario usando SP_DeactivateUser
        /// </summary>
        public async Task<(bool Success, string Message)> DeactivateUserAsync(int userId)
        {
            var inputs = new Dictionary<string, object>
            {
                { "@UserId", userId }
            };

            var parameters = _spExecutor.CreateOutputParameters(inputs);
            var result = await _spExecutor.ExecuteWithOutputAsync("SP_DeactivateUser", parameters);
            return (result.Flag, result.Message);
        }
    }
}
