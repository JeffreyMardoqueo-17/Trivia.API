using System;

namespace TriviaGame.Api.Models
{
    public class GameResult
    {
        public int GameSessionId { get; set; }
        public int TotalScore { get; set; }

        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public int IncorrectAnswers { get; set; }
        public int NotAnswered { get; set; }
    }
}
