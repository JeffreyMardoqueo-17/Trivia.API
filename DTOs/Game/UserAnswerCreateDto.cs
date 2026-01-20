using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TriviaGame.Api.DTOs.Game
{
    public class UserAnswerCreateDto
    {
         public int GameSessionId { get; set; }
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
        public int TimeSpentSeconds { get; set; }
    }
}