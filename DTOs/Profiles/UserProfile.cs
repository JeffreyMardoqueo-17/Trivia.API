using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using TriviaGame.Api.DTOs.Users;
using TriviaGame.Api.Models;

namespace TriviaGame.Api.DTOs.Profiles
{
    public class UserProfile :Profile
    {
        public UserProfile()
        {

        // Para respuesta al cliente
        CreateMap<User, UserResponseDto>();

        // Para crear un usuario
        // Aquí mapeamos solo Gmail; Password ya lo manejamos en el service para hash + salt
        CreateMap<RegisterUserRequestDto, User>()
            .ForMember(dest => dest.Gmail, opt => opt.MapFrom(src => src.Gmail))
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordSalt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        }
    }
}