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
        public int GameSessionId { get; set; }

        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;

        public int TotalScore { get; set; }
        public int Position { get; set; }

        public bool IsTop3 { get; set; }

        public List<RankingDto> Ranking { get; set; } = new();
    }

}