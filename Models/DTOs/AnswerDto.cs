using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TriviaGame.Api.Models.DTOs
{
    /// <summary>
    /// DTO de una respuesta para enviar al cliente
    /// </summary>
    public class AnswerDto
    {
        public int AnswerId { get; set; }
        public string AnswerText { get; set; } = string.Empty;
    }
}