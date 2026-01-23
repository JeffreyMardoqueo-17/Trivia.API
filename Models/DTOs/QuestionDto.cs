using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TriviaGame.Api.Models.DTOs
{
      /// <summary>
    /// DTO de pregunta para enviar al cliente durante el juego
    /// Incluye las respuestas posibles
    /// </summary>
    public class QuestionDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public int Points { get; set; }
        public int TimeLimitSeconds { get; set; }

        public List<AnswerDto> Answers { get; set; } = new();
    }
}