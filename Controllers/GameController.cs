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
    [Route("api/game")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        // 1️⃣ Iniciar sesión de juego
        [HttpPost("start")]
        public async Task<ActionResult<StartGameResponseDto>> StartGame(
            [FromBody] StartGameRequestDto request)
        {
            var gameSessionId =
                await _gameService.StartGameAsync(request.UserId, request.CategoryId);

            return Ok(new StartGameResponseDto
            {
                GameSessionId = gameSessionId
            });
        }

        // 2️⃣ Historial de juegos
        [HttpGet("history/{userId}")]
        public async Task<ActionResult<IEnumerable<GameHistoryDto>>> GetHistory(int userId)
        {
            var history = await _gameService.GetUserGameHistoryAsync(userId);
            return Ok(history);
        }
    }
}
