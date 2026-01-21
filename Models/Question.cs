using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TriviaGame.Api.Models
{/// <summary>
 /// Representa una pregunta de una categoría
 /// </summary>
    public class Question
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Text { get; set; } = string.Empty;
        public int Points { get; set; } = 10;
        public bool IsActive { get; set; } = true;

        // Propiedades de navegación
        public Categories Category { get; set; } = null!;
        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
        public ICollection<GameSessionQuestion> GameSessionQuestions { get; set; } = new List<GameSessionQuestion>();
    }
}
