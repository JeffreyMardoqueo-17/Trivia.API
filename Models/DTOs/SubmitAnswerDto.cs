using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TriviaGame.Api.Models.DTOs
{
    /// <summary>
    /// DTO de respuesta del usuario enviada desde el cliente
    /// </summary>
    public class SubmitAnswerDto
    {
        public int GameSessionId { get; set; }
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
        public int TimeSpentSeconds { get; set; }
    }
}