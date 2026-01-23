using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TriviaGame.Api.Models.DTOs
{
        /// <summary>
    /// Mensaje de error o estado general para el cliente
    /// </summary>
    public class GameMessageDto
    {
        public string Message { get; set; } = string.Empty;
        public bool IsError { get; set; } = false;
    }
}