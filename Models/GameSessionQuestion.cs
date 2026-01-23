using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TriviaGame.Api.Models
{
    
    /// Relaciona una sesión de juego con sus preguntas seleccionadas
    /// </summary>
    public class GameSessionQuestion
    {
        public int Id { get; set; }
        public int GameSessionId { get; set; }
        public int QuestionId { get; set; }
        public int TimeLimitSeconds { get; set; } = 30;

        // Propiedades de navegación
        public GameSession GameSession { get; set; } = null!;
        public Question Question { get; set; } = null!;
    }
}