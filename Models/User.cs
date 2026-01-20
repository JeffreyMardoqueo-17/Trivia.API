using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TriviaGame.Api.Models
{
    /// <summary>
    /// Representa a cada usaurio del sistema
    /// @param Gmail: Correo electronico del usuario
    /// @param PasswordHash: Hash de la contrasena del usuario
    /// @param PasswordSalt: Salt utilizado para hashear la contrasena // lo genero cuando el usuario se registra
    /// </summary>
    public class User
    {
        public int Id { get; set; }
        public string Gmail { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string PasswordSalt { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}