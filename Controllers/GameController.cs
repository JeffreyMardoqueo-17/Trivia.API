using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TriviaGame.Api.DTOs.Game;
using TriviaGame.Api.Models;
using TriviaGame.Api.Services.Interfaces;

namespace TriviaGame.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;
        private readonly IMapper _mapper;

        public GameController(IGameService gameService, IMapper mapper)
        {
            _gameService = gameService;
            _mapper = mapper;
        }

        // ----------------------------
        // Inicia una nueva sesión de juego
        // POST: api/Game/start
        // ----------------------------
        [HttpPost("start")]
        public async Task<ActionResult<StartGameResponseDto>> StartGame([FromBody] StartGameRequestDto request)
        {
            try
            {
                var gameSessionId = await _gameService.StartGameAsync(request.UserId, request.CategoryId);

                var response = new StartGameResponseDto
                {
                    GameSessionId = gameSessionId
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ----------------------------
        // Obtiene preguntas de una sesión
        // GET: api/Game/{gameSessionId}/questions
        // ----------------------------
        [HttpGet("{gameSessionId}/questions")]
        public async Task<ActionResult<IEnumerable<GameQuestionDto>>> GetGameQuestions(int gameSessionId)
        {
            try
            {
                var questions = await _gameService.GetGameQuestionsAsync(gameSessionId);
                var dto = _mapper.Map<IEnumerable<GameQuestionDto>>(questions);

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ----------------------------
        // Obtiene respuestas de una pregunta
        // GET: api/Game/questions/{questionId}/answers
        // ----------------------------
        [HttpGet("questions/{questionId}/answers")]
        public async Task<ActionResult<IEnumerable<AnswerDto>>> GetQuestionAnswers(int questionId)
        {
            try
            {
                var answers = await _gameService.GetQuestionAnswersAsync(questionId);
                var dto = _mapper.Map<IEnumerable<AnswerDto>>(answers);

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ----------------------------
        // Guarda la respuesta del usuario
        // POST: api/Game/answer
        // ----------------------------
        [HttpPost("answer")]
        public async Task<ActionResult> SaveUserAnswer([FromBody] UserAnswerCreateDto request)
        {
            try
            {
                await _gameService.SaveUserAnswerAsync(
                    request.GameSessionId,
                    request.QuestionId,
                    request.AnswerId,
                    request.TimeSpentSeconds
                );

                return Ok(new { message = "Respuesta registrada correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ----------------------------
        // Finaliza una sesión de juego
        // POST: api/Game/{gameSessionId}/end
        // ----------------------------
        [HttpPost("{gameSessionId}/end")]
        public async Task<ActionResult> EndGame(int gameSessionId)
        {
            try
            {
                await _gameService.EndGameAsync(gameSessionId);
                return Ok(new { message = "Juego finalizado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ----------------------------
        // Obtiene historial de juegos de un usuario
        // GET: api/Game/history/{userId}
        // ----------------------------
        [HttpGet("history/{userId}")]
        public async Task<ActionResult<IEnumerable<GameHistoryDto>>> GetUserGameHistory(int userId)
        {
            try
            {
                var history = await _gameService.GetUserGameHistoryAsync(userId);
                var dto = _mapper.Map<IEnumerable<GameHistoryDto>>(history);

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
