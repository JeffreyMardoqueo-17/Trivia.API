using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TriviaGame.Api.Models
{/// <summary>
 /// Representa una sesión de juego de un usuario en una categoría específica
 /// </summary>
    public class GameSession
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }

        public int TotalScore { get; set; } = 0;
        public int MaxDurationSeconds { get; set; } = 90; // 3 preguntas x 30s
        public int TimeSpentSeconds { get; set; } = 0;

        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? EndedAt { get; set; }

        // Propiedades de navegación
        public User User { get; set; } = null!;
        public Categories Category { get; set; } = null!;
        public ICollection<GameSessionQuestion> GameSessionQuestions { get; set; } = new List<GameSessionQuestion>();
        public ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
    }
}