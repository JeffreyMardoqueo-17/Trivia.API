using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using TriviaGame.Api.Data;
using TriviaGame.Api.Models;
using TriviaGame.Api.Models.DTOs;
using TriviaGame.Api.Services.Interfaces;

namespace TriviaGame.Api.Services
{
    public class GameService : IGameService
    {
        private readonly SpExecutor _spExecutor;

        public GameService(SpExecutor spExecutor)
        {
            _spExecutor = spExecutor;
        }

        /// <summary>
        /// Inicia una nueva sesión de juego para un usuario en una categoría
        /// Retorna el GameSessionId (0 si falla)
        /// </summary>
        public async Task<int> StartGameSessionAsync(int userId, int categoryId)
        {
            var parameters = new
            {
                UserId = userId,
                CategoryId = categoryId
            };

            var result = await _spExecutor.QuerySingleAsync<StartGameSessionResult>(
            "SP_StartGameSession",
            parameters
        );

            return result?.GameSessionId ?? 0;
        }


        /// <summary>
        /// Obtiene la siguiente pregunta no respondida junto con sus respuestas
        /// Retorna null si ya no hay preguntas
        /// </summary>
        public async Task<QuestionWithAnswers?> GetNextQuestionAsync(int gameSessionId)
        {
            var result = await _spExecutor.QueryAsync<NextQuestionRaw>(
                "SP_GetNextQuestion",
                new { GameSessionId = gameSessionId }
            );

            if (result == null)
                return null;

            // Mapear la primera pregunta y sus respuestas
            QuestionWithAnswers? question = null;

            foreach (var row in result)
            {
                if (row.QuestionId == null)
                    return null; // ya no hay más preguntas

                if (question == null)
                {
                    question = new QuestionWithAnswers
                    {
                        QuestionId = row.QuestionId.Value,
                        QuestionText = row.QuestionText!,
                        Points = row.Points,
                        TimeLimitSeconds = row.TimeLimitSeconds
                    };
                }

                if (row.AnswerId.HasValue)
                {
                    question.Answers.Add(new Answer
                    {
                        Id = row.AnswerId.Value,
                        QuestionId = row.QuestionId.Value,
                        Text = row.AnswerText!,
                        IsCorrect = false // no revelamos si es correcta aún
                    });
                }
            }

            return question;
        }

        public async Task<AnswerResultDto> SaveUserAnswerAsync(int gameSessionId, int questionId, int answerId, int timeSpentSeconds)
        {
            // Traer la respuesta correcta
            var correctAnswer = await _spExecutor.QuerySingleAsync<Answer>(
      "SP_GetCorrectAnswerByQuestion",
      new { QuestionId = questionId }
  );

            bool isCorrect = correctAnswer != null && correctAnswer.Id == answerId;
            int pointsEarned = isCorrect ? 10 : 0; // cada respuesta correcta 10 puntos

            // Guardar la respuesta del usuario
            var parameters = new DynamicParameters();
            parameters.Add("@GameSessionId", gameSessionId);
            parameters.Add("@QuestionId", questionId);
            parameters.Add("@AnswerId", answerId);
            parameters.Add("@TimeSpentSeconds", timeSpentSeconds);
            parameters.Add("@IsCorrect", isCorrect);
            parameters.Add("@PointsEarned", pointsEarned);

            await _spExecutor.QuerySingleAsync<object>("SP_SaveUserAnswer", parameters);

            return new AnswerResultDto
            {
                IsCorrect = isCorrect,
                PointsEarned = pointsEarned
            };
        }

        public async Task<GameOverDto> EndGameSessionAsync(int gameSessionId)
        {
            // 1inalizar sesión
            await _spExecutor.QuerySingleAsync<object>(
                "SP_EndGameSession",
                new { GameSessionId = gameSessionId }
            );

            // 2️btener info del jugador de la sesión
            var sessionInfo = await _spExecutor.QuerySingleAsync<GameSessionInfoDto>(
                "SP_GetGameSessionInfo",
                new { GameSessionId = gameSessionId }
            );

            // 3Puntaje total
            var totalScore = await _spExecutor.QuerySingleAsync<int>(
                "SP_GetGameSessionTotalScore",
                new { GameSessionId = gameSessionId }
            );

            //  Ranking general
            var ranking = (await _spExecutor
                .QueryAsync<RankingDto>("SP_GetRanking"))
                .ToList();

            //  Calcular posición
            var position = ranking
                .FindIndex(r => r.UserId == sessionInfo.UserId) + 1;

            //  Top 3
            var isTop3 = position > 0 && position <= 3;

            // 7️⃣ DTO FINAL
            return new GameOverDto
            {
                GameSessionId = gameSessionId,
                UserId = sessionInfo.UserId,
                UserName = sessionInfo.UserName,
                TotalScore = totalScore,
                Position = position,
                IsTop3 = isTop3,
                Ranking = ranking
            };
        }

        public async Task<GameSession?> GetGameSessionByIdAsync(int gameSessionId)
        {
            var parameters = new { GameSessionId = gameSessionId };

            return await _spExecutor.QuerySingleAsync<GameSession>(
                "SP_GetGameSessionById",
                parameters
            );
        }

        /// <summary>
        /// Obtiene el ranking general del juego (usuarios ordenados por puntos acumulados)
        /// </summary>
        public async Task<IEnumerable<RankingItem>> GetRankingAsync()
        {
            return await _spExecutor.QueryAsync<RankingItem>("SP_GetRanking");
        }

        public async Task<GameResult> GetGameSessionResultAsync(int gameSessionId)
        {
            return await _spExecutor.QuerySingleAsync<GameResult>(
                "SP_GetGameSessionResult",
                new { GameSessionId = gameSessionId }
            );
        }


        #region Helper Models para mapping interno
        private class NextQuestionRaw
        {
            public int? QuestionId { get; set; }
            public string? QuestionText { get; set; }
            public int Points { get; set; }
            public int TimeLimitSeconds { get; set; }
            public int? AnswerId { get; set; }
            public string? AnswerText { get; set; }
        }

        public class QuestionWithAnswers
        {
            public int QuestionId { get; set; }
            public string QuestionText { get; set; } = string.Empty;
            public int Points { get; set; }
            public int TimeLimitSeconds { get; set; }
            public List<Answer> Answers { get; set; } = new List<Answer>();
        }

        public class RankingItem
        {
            public int UserId { get; set; }
            public string Gmail { get; set; } = string.Empty;
            public int TotalPoints { get; set; }
        }
        #endregion
    }
}
