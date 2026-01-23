using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TriviaGame.Api.Models.DTOs
{
    /// <summary>
    /// DTO de categoría para mostrar en el menú
    /// </summary>
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TotalQuestions { get; set; } // Opcional, para mostrar progreso o validar mínimo de 3 preguntas
    }
}