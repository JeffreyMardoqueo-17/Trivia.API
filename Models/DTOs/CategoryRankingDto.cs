using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TriviaGame.Api.Models.DTOs
{
    public class CategoryRankingDto
    {

        public string Gmail { get; set; } = string.Empty;
        public int TotalPoints { get; set; }
    }
}