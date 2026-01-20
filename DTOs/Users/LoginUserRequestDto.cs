using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace TriviaGame.Api.DTOs.Users
{
   /// <summary>
    /// DTO para el login de un usuario
    /// </summary>
    public class LoginUserRequestDto
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        [MaxLength(100, ErrorMessage = "El correo no puede exceder 100 caracteres")]
        public string Gmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        [MaxLength(50, ErrorMessage = "La contraseña no puede exceder 50 caracteres")]
        public string Password { get; set; } = string.Empty;
    }
}