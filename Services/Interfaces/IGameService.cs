using System.Collections.Generic;
using System.Threading.Tasks;
using TriviaGame.Api.Models; // Asegúrate que las clases estén en este namespace

namespace TriviaGame.Api.Services.Interfaces
{
    public interface IGameService
    {
        // Inicia una sesión de juego y retorna el Id de la sesión creada
        Task<int> StartGameAsync(int userId, int categoryId);

        // Obtiene las preguntas de una sesión de juego
        Task<IEnumerable<GameSessionQuestion>> GetGameQuestionsAsync(int gameSessionId);

        // Obtiene las respuestas de una pregunta específica
        Task<IEnumerable<Answer>> GetQuestionAnswersAsync(int questionId);

        // Guarda la respuesta del usuario en una sesión de juego
        Task SaveUserAnswerAsync(int gameSessionId, int questionId, int answerId, int timeSpentSeconds);

        // Finaliza la sesión de juego
        Task EndGameAsync(int gameSessionId);

        // Obtiene el historial de juegos de un usuario
        Task<IEnumerable<GameSession>> GetUserGameHistoryAsync(int userId);
    }
}
