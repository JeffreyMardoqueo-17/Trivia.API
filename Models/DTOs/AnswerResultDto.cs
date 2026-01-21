using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TriviaGame.Api.Models.DTOs
{
    public class AnswerResultDto
    {
        public bool IsCorrect { get; set; }
        public int PointsEarned { get; set; }
    }
}