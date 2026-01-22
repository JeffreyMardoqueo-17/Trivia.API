using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TriviaGame.Api.Models.DTOs
{
    public class GameSessionInfoDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
    }
}