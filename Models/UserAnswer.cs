using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TriviaGame.Api.Models
{
    public class UserAnswer
    {
        public int Id { get; set; }

        public int GameSessionId { get; set; }
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }

        public bool IsCorrect { get; set; }
        public int PointsEarned { get; set; }
        public int TimeSpentSeconds { get; set; }
        public DateTime AnsweredAt { get; set; }

        public GameSession GameSession { get; set; } = null!;
        public Question Question { get; set; } = null!;
        public Answer Answer { get; set; } = null!;
    }
}