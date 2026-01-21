using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TriviaGame.Api.Models.DTOs
{
    /// <summary>
    /// DTO de ranking para leaderboard
    /// </summary>
    public class RankingDto
    {
        public int UserId { get; set; }
        public string Gmail { get; set; } = string.Empty;
        public int TotalPoints { get; set; }
    }
}