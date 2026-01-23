using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TriviaGame.Api.DTOs.Password
{
    /// <summary>
    /// DTO para restablecer la contrase�a de un usuario
    /// </summary>
    public class ResetPasswordDto
    {
        public string Gmail { get; set; }
        public string? Code { get; set; }
        public string? NewPassword { get; set; }
    }
}