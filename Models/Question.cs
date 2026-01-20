using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TriviaGame.Api.Models
{
    public class Question
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }
        public string Text { get; set; } = string.Empty;
        public int Points { get; set; }
        public bool IsActive { get; set; }

        public Categories Categories { get; set; } = null!;
        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }
}