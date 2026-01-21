using System.Collections.Generic;
using System.Threading.Tasks;
using TriviaGame.Api.Hubs;
using TriviaGame.Api.Models;
using TriviaGame.Api.Models.DTOs;
using TriviaGame.Api.Services;

namespace TriviaGame.Api.Services.Interfaces
{
    public interface IGameService
    {
        /// <summary>
        /// Inicia una nueva sesión de juego para un usuario en una categoría
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="categoryId">ID de la categoría</param>
        /// <returns>ID de la sesión creada, 0 si falla</returns>
        Task<int> StartGameSessionAsync(int userId, int categoryId);

        /// <summary>
        /// Obtiene la siguiente pregunta no respondida junto con sus respuestas
        /// </summary>
        /// <param name="gameSessionId">ID de la sesión de juego</param>
        /// <returns>Objeto con la pregunta y sus respuestas, o null si no hay más preguntas</returns>
        Task<GameService.QuestionWithAnswers?> GetNextQuestionAsync(int gameSessionId);

        /// <summary>
        /// Guarda la respuesta del usuario a una pregunta
        /// </summary>
        /// <param name="gameSessionId">ID de la sesión de juego</param>
        /// <param name="questionId">ID de la pregunta</param>
        /// <param name="answerId">ID de la respuesta elegida</param>
        /// <param name="timeSpentSeconds">Tiempo usado para responder</param>
        Task<TriviaGame.Api.Models.DTOs.AnswerResultDto> SaveUserAnswerAsync(int gameSessionId, int questionId, int answerId, int timeSpentSeconds);

        /// <summary>
        /// Finaliza la sesión de juego
        /// </summary>
        /// <param name="gameSessionId">ID de la sesión</param>
        Task<GameOverDto> EndGameSessionAsync(int gameSessionId);

        /// <summary>
        /// Obtiene el ranking general del juego (usuarios ordenados por puntos acumulados)
        /// </summary>
        Task<IEnumerable<GameService.RankingItem>> GetRankingAsync();

          Task<GameSession?> GetGameSessionByIdAsync(int gameSessionId);
    }
}
