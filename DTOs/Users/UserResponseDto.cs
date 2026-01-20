using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TriviaGame.Api.DTOs.Users
{
    /// <summary>
    /// DTO para la respuesta del login o registro
    /// </summary>
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Gmail { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string Token { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public string Message { get; set; } = string.Empty; // mensaje de operación
        public bool Success { get; set; } // indica si la operación fue correcta
    }
}