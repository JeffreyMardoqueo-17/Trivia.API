using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TriviaGame.Api.Models.DTOs
{
    /// <summary>
    /// DTO para indicar fin de juego
    /// </summary>
    public class GameOverDto
    {
        public int TotalScore { get; set; }
        public List<RankingDto> Ranking { get; set; } = new();
    }
}