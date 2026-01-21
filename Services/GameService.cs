using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TriviaGame.Api.Data;
using TriviaGame.Api.Models;
using TriviaGame.Api.Services.Interfaces;
using Dapper;

namespace TriviaGame.Api.Services
{
    public class GameService : IGameService
    {
        private readonly SpExecutor _spExecutor;

        public GameService(SpExecutor spExecutor)
        {
            _spExecutor = spExecutor;
        }

        // ----------------------------
        // Inicia una sesion de juego 
        // ----------------------------
        public async Task<int> StartGameAsync(int userId, int categoryId)
        {
            // Diccionario con los parámetros de entrada
            var inputs = new Dictionary<string, object>
            {
                { "@UserId", userId }, //id del usuario logueado
                { "@CategoryId", categoryId } //id de la categoría seleccionada
            };

            // Genera el DynamicParameters usando el helper de SpExecutor
            var parameters = _spExecutor.CreateInputParameters(inputs);

            // Ejecuta el SP y obtiene el Id de la sesión
            var result = await _spExecutor.QuerySingleAsync<int>("SP_StartGameSession", parameters);

            if (result == 0)
                throw new Exception("No se pudo iniciar la sesión de juego.");

            return result;
        }

        // Obtiene las preguntas de una sesión de juego
        // ----------------------------
        public async Task<IEnumerable<GameSessionQuestion>> GetGameQuestionsAsync(int gameSessionId)
        {
            // Diccionario con parámetros de entrada
            var inputs = new Dictionary<string, object>
                {
                    { "@GameSessionId", gameSessionId }
                };

            // Genera DynamicParameters usando el helper de SpExecutor
            var parameters = _spExecutor.CreateInputParameters(inputs);

            // Ejecuta el SP y retorna la lista de preguntas
            var questions = await _spExecutor.QueryAsync<GameSessionQuestion>(
                "SP_GetGameQuestions",
                parameters
            );

            return questions;
        }

        // ----------------------------
        // Obtiene las respuestas de una pregunta específica
        // ----------------------------
        public async Task<IEnumerable<Answer>> GetQuestionAnswersAsync(int questionId)
        {
            // Diccionario con parámetros de entrada
            var inputs = new Dictionary<string, object>
            {
                { "@QuestionId", questionId }
            };

            // Genera DynamicParameters usando el helper de SpExecutor
            var parameters = _spExecutor.CreateInputParameters(inputs);

            // Ejecuta el SP y retorna la lista de respuestas
            var answers = await _spExecutor.QueryAsync<Answer>(
                "SP_GetQuestionAnswers",
                parameters
            );

            return answers;
        }

        // ----------------------------
        // Guarda la respuesta del usuario en una sesión de juego
        // ----------------------------
        public async Task SaveUserAnswerAsync(int gameSessionId, int questionId, int answerId, int timeSpentSeconds)
        {
            // Diccionario con parámetros de entrada
            var inputs = new Dictionary<string, object>
            {
                { "@GameSessionId", gameSessionId },
                { "@QuestionId", questionId },
                { "@AnswerId", answerId },
                { "@TimeSpentSeconds", timeSpentSeconds }
            };

            // Genera DynamicParameters usando el helper de SpExecutor
            var parameters = _spExecutor.CreateInputParameters(inputs);

            // Ejecuta el SP
            await _spExecutor.ExecuteWithOutputAsync("SP_SaveUserAnswer", parameters);
        }
        // ----------------------------
        // Finaliza la sesión de juego
        // ----------------------------
        public async Task EndGameAsync(int gameSessionId)
        {
            // Diccionario con parámetros de entrada
            var inputs = new Dictionary<string, object>
            {
                { "@GameSessionId", gameSessionId }
            };

            // Genera DynamicParameters usando el helper de SpExecutor
            var parameters = _spExecutor.CreateInputParameters(inputs);

            // Ejecuta el SP
            await _spExecutor.ExecuteWithOutputAsync("SP_EndGameSession", parameters);
        }

        // ----------------------------
        // Obtiene el historial de juegos de un usuario
        // ----------------------------
        public async Task<IEnumerable<GameSession>> GetUserGameHistoryAsync(int userId)
        {
            // Diccionario con parámetros de entrada
            var inputs = new Dictionary<string, object>
            {
                { "@UserId", userId }
            };

            // Genera DynamicParameters usando el helper de SpExecutor
            var parameters = _spExecutor.CreateInputParameters(inputs);

            // Ejecuta el SP y mapea el resultado a GameSession
            var result = await _spExecutor.QueryAsync<GameSession>("SP_GetUserGameHistory", parameters);

            return result;
        }

        // ----------------------------
        // Obtiene el conteo de preguntas respondidas en una sesión de juego
        // ----------------------------
        public async Task<int> GetAnsweredCountAsync(int gameSessionId)
        {
            var inputs = new Dictionary<string, object>
    {
        { "@GameSessionId", gameSessionId }
    };

            var parameters = _spExecutor.CreateInputParameters(inputs);

            // QuerySingleAsync devuelve un solo valor
            var count = await _spExecutor.QuerySingleAsync<int>("SP_GetAnsweredCount", parameters);

            return count;
        }

        public async Task<NextGameQuestion?> GetNextQuestionAsync(int gameSessionId)
        {
            var rows = (await _spExecutor.QueryAsync<NextQuestionRow>(
                "SP_GetNextQuestion",
                new { GameSessionId = gameSessionId }
            )).ToList();

            if (!rows.Any())
                return null;

            var first = rows.First();

            return new NextGameQuestion
            {
                QuestionId = first.QuestionId,
                QuestionText = first.QuestionText,
                Points = first.Points,
                TimeLimitSeconds = first.TimeLimitSeconds,
                Answers = rows.Select(r => new Answer
                {
                    Id = r.AnswerId,
                    Text = r.AnswerText
                }).ToList()
            };
        }



    }
}
