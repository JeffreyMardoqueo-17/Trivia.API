using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TriviaGame.Api.DTOs.Game
{
    public class NextGameQuestionDto
    {
        public int QuestionId { get; set; }
    public string QuestionText { get; set; } = "";
    public int Points { get; set; }
    public int TimeLimitSeconds { get; set; }
    public List<AnswerOptionDto> Answers { get; set; } = new();
    }
}