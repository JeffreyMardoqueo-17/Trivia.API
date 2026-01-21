using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TriviaGame.Api.DTOs.Game
{
    public class AnswerOptionDto
    {
        public int AnswerId { get; set; }
        public string AnswerText { get; set; } = string.Empty;

    }
}