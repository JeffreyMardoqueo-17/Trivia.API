using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TriviaGame.Api.DTOs.Game
{
    public class StartGameRequestDto
    {
         public int UserId { get; set; }
        public int CategoryId { get; set; }
    }
}