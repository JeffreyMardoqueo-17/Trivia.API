using Microsoft.AspNetCore.SignalR;
using TriviaGame.Api.Services.Interfaces;

public class GameHub : Hub
{
    private readonly IGameService _gameService;

    public GameHub(IGameService gameService)
    {
        _gameService = gameService;
    }

    // 1️⃣ El cliente entra al juego
    public async Task StartGame(int gameSessionId)
    {

        Console.WriteLine($"[HUB] StartGame recibido. SessionId={gameSessionId}");
        Console.WriteLine($"Client connected: {Context.ConnectionId} to game session {gameSessionId}");
        // Usaremos SP para obtener la siguiente pregunta
        var question = await _gameService.GetNextQuestionAsync(gameSessionId);

        if (question == null)
        {
             Console.WriteLine("[HUB] GetNextQuestionAsync devolvió NULL");
            await Clients.Caller.SendAsync("GameEnded");
            return;
        }

         Console.WriteLine($"[HUB] Enviando pregunta {question.QuestionId}");
        await Clients.Caller.SendAsync("ReceiveQuestion", question);
    }

    // 2️⃣ El cliente responde
    public async Task SubmitAnswer(
        int gameSessionId,
        int questionId,
        int answerId,
        int timeSpentSeconds
    )
    {
        // Guarda usando SP
        await _gameService.SaveUserAnswerAsync(
            gameSessionId,
            questionId,
            answerId,
            timeSpentSeconds
        );

        // Busca la siguiente pregunta
        var nextQuestion = await _gameService.GetNextQuestionAsync(gameSessionId);

        if (nextQuestion == null)
        {
            await _gameService.EndGameAsync(gameSessionId);
            await Clients.Caller.SendAsync("GameEnded");
            return;
        }

        await Clients.Caller.SendAsync("ReceiveQuestion", nextQuestion);
    }
}
