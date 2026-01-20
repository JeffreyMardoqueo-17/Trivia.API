using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using TriviaGame.Api.Common;

namespace TriviaGame.Api.Middleware
{
       public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            // ===== 1️⃣ CAPTURA CONTEXTO DEL REQUEST =====
            var request = context.Request;

            var endpoint = $"{request.Method} {request.Path}";
            var queryString = request.QueryString.ToString();
            var traceId = context.TraceIdentifier;

            // ===== USUARIO (SI EXISTE) =====
            var userId = context.User?.Claims?
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var userName = context.User?.Identity?.Name;

            // ===== LOG COMPLETO (observacion completa ) =====
            _logger.LogError(ex,
                "Unhandled exception occurred. " +
                "Endpoint: {Endpoint} | Query: {Query} | UserId: {UserId} | UserName: {UserName} | TraceId: {TraceId}",
                endpoint,
                queryString,
                userId ?? "Anonymous",
                userName ?? "Anonymous",
                traceId
            );

            // =====  RESPUESTA HTTP =====
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = GetStatusCode(ex);

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = GetClientMessage(ex),
                TraceId = traceId,
                Timestamp = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }

        private static int GetStatusCode(Exception ex)
        {
            return ex switch
            {
                BusinessException => StatusCodes.Status400BadRequest,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                ArgumentException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };
        }

        private static string GetClientMessage(Exception ex)
        {
            return ex switch
            {
                BusinessException => ex.Message,
                KeyNotFoundException => "El recurso solicitado no existe.",
                UnauthorizedAccessException => "No autorizado.",
                ArgumentException => ex.Message,
                _ => "Ocurrió un error inesperado en el servidor."
            };
        }
    }

}