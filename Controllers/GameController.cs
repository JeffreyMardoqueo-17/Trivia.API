using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TriviaGame.Api.Models.DTOs;
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

        /// <summary>
        /// Inicia una nueva sesión de juego para un usuario y categoría
        /// </summary>
        [HttpPost("start")]
        public async Task<ActionResult<GameSessionDto>> Start([FromQuery] int userId, [FromQuery] int categoryId)
        {
            // Llamamos al service para crear la sesión
            var gameSessionId = await _gameService.StartGameSessionAsync(userId, categoryId);
            if (gameSessionId == 0)
                return BadRequest(new { Message = "No se pudo iniciar la sesión. Verifica usuario y categoría." });

            // Obtenemos la sesión completa para mapear
            var gameSession = await _gameService.GetGameSessionByIdAsync(gameSessionId);
            if (gameSession == null)
                return NotFound(new { Message = "Sesión no encontrada." });

            // Mapear a DTO
            var gameSessionDto = _mapper.Map<GameSessionDto>(gameSession);
            return Ok(gameSessionDto);
        }

        /// <summary>
        /// Obtiene la siguiente pregunta de la sesión
        /// </summary>
        [HttpGet("{gameSessionId}/next-question")]
        public async Task<ActionResult<QuestionDto>> GetNextQuestion(int gameSessionId)
        {
            var questionInternal = await _gameService.GetNextQuestionAsync(gameSessionId);
            if (questionInternal == null)
                return NotFound(new { Message = "No hay más preguntas." });

            var questionDto = _mapper.Map<QuestionDto>(questionInternal);
            return Ok(questionDto);
        }

        /// <summary>
        /// Envía la respuesta del usuario
        /// </summary>
        [HttpPost("submit-answer")]
        public async Task<ActionResult<AnswerResultDto>> SubmitAnswer([FromBody] SubmitAnswerDto submitAnswer)
        {
            var result = await _gameService.SaveUserAnswerAsync(
                submitAnswer.GameSessionId,
                submitAnswer.QuestionId,
                submitAnswer.AnswerId,
                submitAnswer.TimeSpentSeconds
            );

            return Ok(result);
        }

        /// <summary>
        /// Finaliza la sesión de juego
        /// </summary>
        [HttpPost("{gameSessionId}/end")]
        public async Task<ActionResult<GameOverDto>> End(int gameSessionId)
        {
            var gameOver = await _gameService.EndGameSessionAsync(gameSessionId);
            return Ok(gameOver);
        }

        /// <summary>
        /// Obtiene el ranking general
        /// </summary>
        [HttpGet("ranking")]
        public async Task<ActionResult<List<RankingDto>>> GetRanking()
        {
            var rankingInternal = await _gameService.GetRankingAsync();
            var rankingDto = _mapper.Map<List<RankingDto>>(rankingInternal);
            return Ok(rankingDto);
        }
    }
}
