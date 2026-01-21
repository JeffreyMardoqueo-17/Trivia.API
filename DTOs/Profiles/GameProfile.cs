using AutoMapper;
using TriviaGame.Api.Models;
using TriviaGame.Api.DTOs;
using TriviaGame.Api.DTOs.Game;

namespace TriviaGame.Api.DTOs.Profiles
{
    public class GameProfile : Profile
    {
        public GameProfile()
        {
            // -------------------------
            // GameSession -> StartGameResponseDto
            // -------------------------
            CreateMap<GameSession, StartGameResponseDto>()
                .ForMember(dest => dest.GameSessionId, opt => opt.MapFrom(src => src.Id));

            // -------------------------
            // GameSessionQuestion -> GameQuestionDto
            // -------------------------
            CreateMap<GameSessionQuestion, GameQuestionDto>()
             .ForMember(dest => dest.QuestionId, opt => opt.MapFrom(src => src.QuestionId))
             .ForMember(dest => dest.QuestionText, opt => opt.MapFrom(src => src.QuestionText)) // usa la columna del SP
             .ForMember(dest => dest.Points, opt => opt.MapFrom(src => src.Points))
             .ForMember(dest => dest.TimeLimitSeconds, opt => opt.MapFrom(src => src.TimeLimitSeconds));

            // -------------------------
            // Answer -> AnswerDto
            // -------------------------
            CreateMap<Answer, AnswerDto>()
                .ForMember(dest => dest.AnswerId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AnswerText, opt => opt.MapFrom(src => src.Text));

            // -------------------------
            // UserAnswerCreateDto -> UserAnswer
            // -------------------------
            CreateMap<UserAnswerCreateDto, UserAnswer>();

            // -------------------------
            // GameSession -> GameHistoryDto
            // -------------------------
            CreateMap<GameSession, GameHistoryDto>()
      .ForMember(dest => dest.GameSessionId, opt => opt.MapFrom(src => src.Id))
      .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.CategoryName));

        }
    }
}
