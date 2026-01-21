using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TriviaGame.Api.Models
{
    public class GameSessionQuestion
    {
        public int Id { get; set; }
        public int GameSessionId { get; set; }
        public int QuestionId { get; set; }
        public int TimeLimitSeconds { get; set; }

        public GameSession GameSession { get; set; } = null!;
        public Question Question { get; set; } = null!;
    }
}