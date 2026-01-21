using Microsoft.AspNetCore.SignalR;
using TriviaGame.Api.DTOs.Game;
using TriviaGame.Api.Models;
using TriviaGame.Api.Services.Interfaces;
using AutoMapper;

public class GameHub : Hub
{
    private readonly IGameService _gameService;
    private readonly IMapper _mapper;

    public GameHub(IGameService gameService, IMapper mapper)
    {
        _gameService = gameService;
        _mapper = mapper;
    }

    // ▶️ INICIAR JUEGO
    public async Task StartGame(int gameSessionId)
    {
        await SendNextQuestion(gameSessionId);
    }

    // ▶️ RESPONDER PREGUNTA
    public async Task SubmitAnswer(
        int gameSessionId,
        int questionId,
        int? answerId, // nullable, para cuando el tiempo se agote
        int timeSpentSeconds
    )
    {
        // Guardar la respuesta del usuario
        await _gameService.SaveUserAnswerAsync(
            gameSessionId,
            questionId,
            answerId ?? 0, // si es null, se envía 0
            timeSpentSeconds
        );

        // Enviar la siguiente pregunta o terminar juego
        await SendNextQuestion(gameSessionId);
    }

    // ▶️ FLUJO CENTRAL DEL JUEGO
    // ▶️ FLUJO CENTRAL DEL JUEGO
    private async Task SendNextQuestion(int gameSessionId)
    {
        var question = await _gameService.GetNextQuestionAsync(gameSessionId);

        if (question == null)
        {
            await _gameService.EndGameAsync(gameSessionId);
            await Clients.Caller.SendAsync("GameEnded");
            return;
        }

        // ✅ Usar AutoMapper para mapear la pregunta y sus respuestas
        var dto = _mapper.Map<NextGameQuestionDto>(question);

        // 🔍 LOG para debug → aquí verificamos lo que realmente se envía
        Console.WriteLine("📩 SendNextQuestion DTO RAW: " + System.Text.Json.JsonSerializer.Serialize(dto));

        // Debug para saber qué se envía
        Console.WriteLine($"📩 Enviando pregunta {dto.QuestionId}: {dto.QuestionText}");
        dto.Answers.ForEach(a =>
            Console.WriteLine($"    ➡️ Respuesta {a.AnswerId}: {a.AnswerText}")
        );

        await Clients.Caller.SendAsync("ReceiveQuestion", dto);
    }

}
