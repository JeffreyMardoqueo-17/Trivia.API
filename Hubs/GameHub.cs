using Microsoft.AspNetCore.SignalR;
using TriviaGame.Api.Models.DTOs;
using TriviaGame.Api.Services.Interfaces;
using AutoMapper;

namespace TriviaGame.Api.Hubs
{
    public class GameHub : Hub
    {
        private readonly IGameService _gameService;
        private readonly IMapper _mapper;

        public GameHub(IGameService gameService, IMapper mapper)
        {
            _gameService = gameService;
            _mapper = mapper;
        }

        /// <summary>
        /// Inicia una nueva sesión de juego para un usuario en una categoría
        /// Devuelve el GameSessionDto al cliente
        /// </summary>
        public async Task<GameSessionDto?> StartGameSession(int userId, int categoryId)
        {
            var gameSessionId = await _gameService.StartGameSessionAsync(userId, categoryId);
            if (gameSessionId == 0) return null;

            // Obtenemos la sesión para mapear
            // Si tu SP StartGameSession ya devuelve todos los campos, úsalo; sino crea un método en service para obtener GameSession
            // Aquí asumimos que el service tiene un método GetGameSessionByIdAsync
            var gameSession = await _gameService.GetGameSessionByIdAsync(gameSessionId);
            return _mapper.Map<GameSessionDto>(gameSession);
        }

        /// <summary>
        /// Obtiene la siguiente pregunta de la sesión en curso
        /// </summary>
        public async Task<QuestionDto?> GetNextQuestion(int gameSessionId)
        {
            var questionInternal = await _gameService.GetNextQuestionAsync(gameSessionId);
            if (questionInternal == null) return null;

            // Mapear a DTO
            var questionDto = _mapper.Map<QuestionDto>(questionInternal);
            return questionDto;
        }

        /// <summary>
        /// Guarda la respuesta del usuario
        /// Devuelve si fue correcta y los puntos obtenidos
        /// </summary>
        public async Task<AnswerResultDto> SubmitAnswer(SubmitAnswerDto submitAnswer)
        {
            // Guardamos la respuesta usando el service
            var result = await _gameService.SaveUserAnswerAsync(
                submitAnswer.GameSessionId,
                submitAnswer.QuestionId,
                submitAnswer.AnswerId,
                submitAnswer.TimeSpentSeconds
            );

            // Mapear a DTO para enviar al cliente
            var answerResult = new AnswerResultDto
            {
                IsCorrect = result.IsCorrect,
                PointsEarned = result.PointsEarned,
                TimeSpentSeconds = submitAnswer.TimeSpentSeconds
            };

            return answerResult;
        }

        /// <summary>
        /// Finaliza la sesión de juego y devuelve el puntaje final + ranking
        /// </summary>
        public async Task<GameOverDto> EndGame(int gameSessionId)
        {
            var gameOverInternal = await _gameService.EndGameSessionAsync(gameSessionId);
            return _mapper.Map<GameOverDto>(gameOverInternal);
        }

        /// <summary>
        /// Devuelve el ranking general de todos los usuarios
        /// </summary>
        public async Task<List<RankingDto>> GetRanking()
        {
            var rankingInternal = await _gameService.GetRankingAsync();
            return _mapper.Map<List<RankingDto>>(rankingInternal);
        }
    }

    // DTO de resultado de respuesta que enviaremos al cliente
    public class AnswerResultDto
    {
        public bool IsCorrect { get; set; }
        public int PointsEarned { get; set; }
        public int TimeSpentSeconds { get; set; }
    }
}
