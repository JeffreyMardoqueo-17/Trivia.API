using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TriviaGame.Api.Models
{
    public class NextGameQuestionWithAnswer
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public int Points { get; set; }
        public int TimeLimitSeconds { get; set; }

        public int AnswerId { get; set; }
        public string AnswerText { get; set; } = string.Empty;
    }
}