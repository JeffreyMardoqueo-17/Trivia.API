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
             .ForMember(dest => dest.QuestionText, opt => opt.MapFrom(src => src.Question)) // usa la columna del SP
            //  .ForMember(dest => dest.Points, opt => opt.MapFrom(src => src.Points))
             .ForMember(dest => dest.TimeLimitSeconds, opt => opt.MapFrom(src => src.TimeLimitSeconds));

            // -------------------------
            // Answer -> AnswerDto
            // -------------------------
            CreateMap<Answer, AnswerOptionDto>()
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
      .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Categories));

            // Mapear NextGameQuestion → NextGameQuestionDto
            CreateMap<NextGameQuestion, NextGameQuestionDto>()
                .ForMember(dest => dest.QuestionId, opt => opt.MapFrom(src => src.QuestionId))
                .ForMember(dest => dest.QuestionText, opt => opt.MapFrom(src => src.QuestionText))
                .ForMember(dest => dest.Points, opt => opt.MapFrom(src => src.Points))
                .ForMember(dest => dest.TimeLimitSeconds, opt => opt.MapFrom(src => src.TimeLimitSeconds))
                .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Answers));

            // Mapear Answer → AnswerOptionDto
           CreateMap<Answer, AnswerOptionDto>()
            .ForMember(dest => dest.AnswerId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.AnswerText, opt => opt.MapFrom(src => src.Text));
        }
    }
}
