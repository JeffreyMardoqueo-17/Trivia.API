using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TriviaGame.Api.Models.DTOs
{
    /// <summary>
    /// DTO usado para enviar el estado de la sesión de juego al cliente
    /// </summary>
    public class GameSessionDto
    {
        public int GameSessionId { get; set; }
        public int CategoryId { get; set; }
        public int TotalScore { get; set; }
        public int MaxDurationSeconds { get; set; }
        public int TimeSpentSeconds { get; set; }
        public DateTime StartedAt { get; set; }
    }
}