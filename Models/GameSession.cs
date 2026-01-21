using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TriviaGame.Api.Models
{
    public class GameSession
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty; // <- nueva
        public int TotalScore { get; set; }
        public int TimeSpentSeconds { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }

        public Categories Categories { get; set; } = null!;
        public ICollection<GameSessionQuestion> GameSessionQuestions { get; set; } = new List<GameSessionQuestion>();
        public ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
    }

}