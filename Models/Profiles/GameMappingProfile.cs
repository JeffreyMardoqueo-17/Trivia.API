using AutoMapper;
using System.Linq;
using TriviaGame.Api.Models;
using TriviaGame.Api.Models.DTOs;
using TriviaGame.Api.Services;

namespace TriviaGame.Api.Models.Profiles
{
    public class GameMappingProfile : Profile
    {
        public GameMappingProfile()
        {
            // GameSession -> GameSessionDto
            CreateMap<GameSession, GameSessionDto>()
                .ForMember(dest => dest.GameSessionId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.TotalScore, opt => opt.MapFrom(src => src.TotalScore))
                .ForMember(dest => dest.MaxDurationSeconds, opt => opt.MapFrom(src => src.MaxDurationSeconds))
                .ForMember(dest => dest.TimeSpentSeconds, opt => opt.MapFrom(src => src.TimeSpentSeconds))
                .ForMember(dest => dest.StartedAt, opt => opt.MapFrom(src => src.StartedAt));

            // Categories -> CategoryDto
            CreateMap<Categories, CategoryDto>()
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TotalQuestions, opt => opt.MapFrom(src => src.Questions.Count));

            // Answer -> AnswerDto
            CreateMap<Answer, AnswerDto>()
                .ForMember(dest => dest.AnswerId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AnswerText, opt => opt.MapFrom(src => src.Text));

            // Question -> QuestionDto
            // Incluye respuestas filtrando IsActive (opcional)
            CreateMap<Question, QuestionDto>()
                .ForMember(dest => dest.QuestionId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.QuestionText, opt => opt.MapFrom(src => src.Text))
                .ForMember(dest => dest.Points, opt => opt.MapFrom(src => src.Points))
                .ForMember(dest => dest.TimeLimitSeconds, opt => opt.Ignore()) // Lo llenamos desde GameSessionQuestion
                .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Answers));

            // GameService.QuestionWithAnswers -> QuestionDto
            // Para SPs que retornan preguntas + respuestas
            CreateMap<GameService.QuestionWithAnswers, QuestionDto>()
                .ForMember(dest => dest.QuestionId, opt => opt.MapFrom(src => src.QuestionId))
                .ForMember(dest => dest.QuestionText, opt => opt.MapFrom(src => src.QuestionText))
                .ForMember(dest => dest.Points, opt => opt.MapFrom(src => src.Points))
                .ForMember(dest => dest.TimeLimitSeconds, opt => opt.MapFrom(src => src.TimeLimitSeconds))
                .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Answers));

            // GameService.RankingItem -> RankingDto
            CreateMap<GameService.RankingItem, RankingDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Gmail, opt => opt.MapFrom(src => src.Gmail))
                .ForMember(dest => dest.TotalPoints, opt => opt.MapFrom(src => src.TotalPoints));

            // SubmitAnswerDto -> UserAnswer (opcional)
            //solo para apear la respuesta del cliente al modelo interno
            CreateMap<SubmitAnswerDto, UserAnswer>()
                .ForMember(dest => dest.GameSessionId, opt => opt.MapFrom(src => src.GameSessionId))
                .ForMember(dest => dest.QuestionId, opt => opt.MapFrom(src => src.QuestionId))
                .ForMember(dest => dest.AnswerId, opt => opt.MapFrom(src => src.AnswerId))
                .ForMember(dest => dest.TimeSpentSeconds, opt => opt.MapFrom(src => src.TimeSpentSeconds));

            // GameOverDto ->
            CreateMap<(int totalScore, List<RankingDto> ranking), GameOverDto>()
                .ForMember(dest => dest.TotalScore, opt => opt.MapFrom(src => src.totalScore))
                .ForMember(dest => dest.Ranking, opt => opt.MapFrom(src => src.ranking));
            //
            // Game reuslt => Game resultdto => para mostrar el resultado final de una partida 
            CreateMap<GameResult, GameResultDto>();

             CreateMap<CategoryRankingItem, CategoryRankingDto>();
        }
    }
}
